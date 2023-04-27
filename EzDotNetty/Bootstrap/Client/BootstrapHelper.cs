using DotNetty.Codecs;
using DotNetty.Handlers.Logging;
using DotNetty.Handlers.Tls;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using EzDotNetty.Config;
using EzDotNetty.Handler.Client;
using EzDotNetty.Logging;
using Serilog;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace EzDotNetty.Bootstrap.Client
{
    public class BootstrapHelper
    {
        private static MultithreadEventLoopGroup? EventLoopGroup;

        public static async Task<IChannel> RunClientAsync<THandler>(Action<THandler>? action = null)
            where THandler : NetworkHandler, new()
        {
            Loggers.Initialize();

            EventLoopGroup = new MultithreadEventLoopGroup();

            X509Certificate2? cert = null;
            string? targetHost = null;
            if (Config.Client.Settings.IsSsl)
            {
                cert = new X509Certificate2(Path.Combine(Helper.ProcessDirectory, "dotnetty.com.pfx"), "password");
                targetHost = cert.GetNameInfo(X509NameType.DnsName, false);
            }

            var handler = new THandler();

            var bootstrap = new DotNetty.Transport.Bootstrapping.Bootstrap();
            bootstrap
                .Group(EventLoopGroup)
                .Channel<TcpSocketChannel>()
                .Option(ChannelOption.TcpNodelay, true)
                .Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
                {
                    var pipeline = channel.Pipeline;

                    if (cert != null)
                    {
                        pipeline.AddLast("tls", new TlsHandler(stream => new SslStream(stream, true, (sender, certificate, chain, errors) => true), new ClientTlsSettings(targetHost)));
                    }
                    pipeline.AddLast(new LoggingHandler());
                    pipeline.AddLast("framing-enc", new LengthFieldPrepender(4));
                    pipeline.AddLast("framing-dec", new LengthFieldBasedFrameDecoder(ushort.MaxValue, 0, 4, 0, 4));

                    action?.Invoke(handler);

                    pipeline.AddLast("handler", handler);
                }));

            var clientChannel = await bootstrap.ConnectAsync(new IPEndPoint(Config.Client.Settings.Host, Config.Client.Settings.Port));

            Log.Logger.Information("Started Client");
            return clientChannel;
        }

        public static async Task GracefulCloseAsync()
        {
            await EventLoopGroup!.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));
        }
    }
}
