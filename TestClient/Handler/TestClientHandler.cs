using DotNetty.Buffers;
using DotNetty.Transport.Channels;
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

            Loggers.Message!.Information($"OnChannelActive:{context.Channel.Id}");
        }

        public override void OnChannelUnregistered(IChannelHandlerContext context)
        {
            Loggers.Message!.Information($"OnChannelUnregistered:{context.Channel.Id}");
        }

        public override void OnReceive(IChannelHandlerContext context, string str)
        {
            Loggers.Message!.Information(str);

            Thread.Sleep(1000);

            var msg = Unpooled.Buffer();
            msg.WriteString($"OnReceive:{context.Channel.Id}", System.Text.Encoding.UTF8);
            context.WriteAndFlushAsync(msg);
        }
    }
}
