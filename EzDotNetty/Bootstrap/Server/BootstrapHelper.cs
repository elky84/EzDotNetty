﻿using DotNetty.Codecs;
using DotNetty.Handlers.Logging;
using DotNetty.Handlers.Tls;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using DotNetty.Transport.Libuv;
using EzDotNetty.Config;
using EzDotNetty.Logging;
using Serilog;
using System.Security.Cryptography.X509Certificates;

namespace EzDotNetty.Bootstrap.Server
{
    public class BootstrapHelper
    {
        private static IEventLoopGroup? BossGroup;

        private static IEventLoopGroup? WorkerGroup;

        public static async Task<IChannel> RunServerAsync<THandler>()
            where THandler : ChannelHandlerAdapter, new()
        {
            Loggers.Initialize();

            if (Config.Server.Settings.UseLibuv)
            {
                var dispatcher = new DispatcherEventLoopGroup();
                BossGroup = dispatcher;
                WorkerGroup = new WorkerEventLoopGroup(dispatcher);
            }
            else
            {
                BossGroup = new MultithreadEventLoopGroup(1);
                WorkerGroup = new MultithreadEventLoopGroup();
            }

            X509Certificate2? tlsCertificate = null;
            if (Config.Server.Settings.IsSsl)
            {
                tlsCertificate = new X509Certificate2(Path.Combine(Helper.ProcessDirectory, "dotnetty.com.pfx"), "password");
            }

            var bootstrap = new ServerBootstrap();
            bootstrap.Group(BossGroup, WorkerGroup);

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

                    pipeline.AddLast("handler", new THandler());
                }));

            IChannel boundChannel = await bootstrap.BindAsync(Config.Server.Settings.Port);

            Log.Logger.Information("Started Server");

            return boundChannel;
        }

        public static async Task GracefulCloseAsync()
        {
            await Task.WhenAll(
                BossGroup!.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)),
                WorkerGroup!.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)));
        }
    }
}
