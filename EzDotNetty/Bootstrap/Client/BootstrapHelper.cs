using DotNetty.Codecs;
using DotNetty.Handlers.Logging;
using DotNetty.Handlers.Tls;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using EzDotNetty.Config;
using System.Net;
using Serilog;
using EzDotNetty.Logging;
using EzDotNetty.Handler.Client;

namespace EzDotNetty.Bootstrap.Client
{
    public class BootstrapHelper
    {
        static public async Task RunClientAsync<THandler>(Action<THandler>? action = null) 
            where THandler : NetworkHandler, new()
        {
            Log.Logger = new LoggerConfiguration()
                            .MinimumLevel.Information()
                            .WriteTo.Console()
                            .CreateLogger();

            Collection.Init<LoggerId>();

            var group = new MultithreadEventLoopGroup();

            X509Certificate2? cert = null;
            string? targetHost = null;
            if (Config.Client.Settings.IsSsl)
            {
                cert = new X509Certificate2(Path.Combine(Helper.ProcessDirectory, "dotnetty.com.pfx"), "password");
                targetHost = cert.GetNameInfo(X509NameType.DnsName, false);
            }
            try
            {
                var bootstrap = new DotNetty.Transport.Bootstrapping.Bootstrap();
                bootstrap
                    .Group(group)
                    .Channel<TcpSocketChannel>()
                    .Option(ChannelOption.TcpNodelay, true)
                    .Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
                    {
                        IChannelPipeline pipeline = channel.Pipeline;

                        if (cert != null)
                        {
                            pipeline.AddLast("tls", new TlsHandler(stream => new SslStream(stream, true, (sender, certificate, chain, errors) => true), new ClientTlsSettings(targetHost)));
                        }
                        pipeline.AddLast(new LoggingHandler());
                        pipeline.AddLast("framing-enc", new LengthFieldPrepender(4));
                        pipeline.AddLast("framing-dec", new LengthFieldBasedFrameDecoder(ushort.MaxValue, 0, 4, 0, 4));

                        var handler = new THandler();
                        action?.Invoke(handler);

                        pipeline.AddLast("handler", handler);
                    }));

                IChannel clientChannel = await bootstrap.ConnectAsync(new IPEndPoint(Config.Client.Settings.Host, Config.Client.Settings.Port));

                Collection.Get(LoggerId.Message)!.Information("Started Client");

                Console.ReadLine();

                await clientChannel.CloseAsync();
            }
            finally
            {
                await group.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));
            }
        }
    }
}
