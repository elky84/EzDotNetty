using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using EzDotNetty.Handler.Server;
using EzDotNetty.Log;

namespace TestServer.Handler
{
    public class TestServerHandler : NetworkHandler
    {
        public override void OnChannelActive(IChannelHandlerContext context)
        {
            Loggers.Message!.Information($"OnChannelActive:{context.Channel.Id}");
        }

        public override void OnChannelInactive(IChannelHandlerContext context)
        {
            Loggers.Message!.Information($"OnChannelInactive:{context.Channel.Id}");
        }

        public override void OnReceive(IChannelHandlerContext context, string str)
        {
            Loggers.Message!.Information(str);

            var msg = Unpooled.Buffer();
            msg.WriteString($"OnReceive:{context.Channel.Id}", System.Text.Encoding.UTF8);
            context.WriteAndFlushAsync(msg);
        }
    }
}
