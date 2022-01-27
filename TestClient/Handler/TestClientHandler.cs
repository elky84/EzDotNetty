﻿using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using EzDotnetty.Logging;
using EzDotNetty.Handler.Client;
using EzDotNetty.Logging;
using System.Threading;

namespace TestClient.Handler
{
    public class TestClientHandler : NetworkHandler
    {
        public override void OnChannelActive(IChannelHandlerContext context)
        {
            var msg = Unpooled.Buffer();
            msg.WriteString("OnConnect Client", System.Text.Encoding.UTF8);
            context.WriteAndFlushAsync(msg);

            Collection.Get(LoggerId.Message)!.Information($"OnChannelActive:{context.Channel.Id}");
        }

        public override void OnChannelUnregistered(IChannelHandlerContext context)
        {
            Collection.Get(LoggerId.Message)!.Information($"OnChannelUnregistered:{context.Channel.Id}");
        }

        public override void OnReceive(IChannelHandlerContext context, string str)
        {
            Collection.Get(LoggerId.Message)!.Information(str);

            Thread.Sleep(1000);

            var msg = Unpooled.Buffer();
            msg.WriteString($"OnReceive:{context.Channel.Id}", System.Text.Encoding.UTF8);
            context.WriteAndFlushAsync(msg);
        }
    }
}
