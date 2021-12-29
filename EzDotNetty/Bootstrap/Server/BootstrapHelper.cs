using DotNetty.Codecs;
using DotNetty.Handlers.Logging;
using DotNetty.Handlers.Tls;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using DotNetty.Transport.Libuv;
using EzDotNetty.Config;
using Serilog;
using System.Security.Cryptography.X509Certificates;

namespace EzDotNetty.Bootstrap.Server
{
    public class BootstrapHelper
    {
        static public async Task RunServerAsync<T>() where T : ChannelHandlerAdapter, new()
        {
            Serilog.Log.Logger = new LoggerConfiguration()
                            .MinimumLevel.Information()
                            .WriteTo.Console()
                            .CreateLogger();

            Log.Loggers.Init();

            IEventLoopGroup bossGroup;
            IEventLoopGroup workerGroup;

            if (Config.Server.Settings.UseLibuv)
            {
                var dispatcher = new DispatcherEventLoopGroup();
                bossGroup = dispatcher;
                workerGroup = new WorkerEventLoopGroup(dispatcher);
            }
            else
            {
                bossGroup = new MultithreadEventLoopGroup(1);
                workerGroup = new MultithreadEventLoopGroup();
            }

            X509Certificate2? tlsCertificate = null;
            if (Config.Server.Settings.IsSsl)
            {
                tlsCertificate = new X509Certificate2(Path.Combine(Helper.ProcessDirectory, "dotnetty.com.pfx"), "password");
            }
            try
            {
                var bootstrap = new ServerBootstrap();
                bootstrap.Group(bossGroup, workerGroup);

                if (Config.Server.Settings.UseLibuv)
                {
                    bootstrap.Channel<TcpServerChannel>();
                }
                else
                {
                    bootstrap.Channel<TcpServerSocketChannel>();
                }

                bootstrap
                    .Option(ChannelOption.SoBacklog, 100)
                    .Handler(new LoggingHandler("SRV-LSTN"))
                    .ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
                    {
                        IChannelPipeline pipeline = channel.Pipeline;
                        if (tlsCertificate != null)
                        {
                            pipeline.AddLast("tls", TlsHandler.Server(tlsCertificate));
                        }
                        pipeline.AddLast(new LoggingHandler("SRV-CONN"));
                        pipeline.AddLast("framing-enc", new LengthFieldPrepender(4));
                        pipeline.AddLast("framing-dec", new LengthFieldBasedFrameDecoder(ushort.MaxValue, 0, 4, 0, 4));

                        pipeline.AddLast("handler", new T());
                    }));

                IChannel boundChannel = await bootstrap.BindAsync(Config.Server.Settings.Port);

                Log.Loggers.Message!.Information("Started Server");

                Console.ReadLine();

                await boundChannel.CloseAsync();
            }
            finally
            {
                await Task.WhenAll(
                    bossGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)),
                    workerGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)));
            }

        }
    }
}
