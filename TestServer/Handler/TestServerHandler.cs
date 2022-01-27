using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using EzDotnetty.Logging;
using EzDotNetty.Handler.Server;
using EzDotNetty.Logging;

namespace TestServer.Handler
{
    public class TestServerHandler : NetworkHandler
    {
        public override void OnChannelActive(IChannelHandlerContext context)
        {
            Collection.Get(LoggerId.Message)!.Information($"OnChannelActive:{context.Channel.Id}");
        }

        public override void OnChannelInactive(IChannelHandlerContext context)
        {
            Collection.Get(LoggerId.Message)!.Information($"OnChannelInactive:{context.Channel.Id}");
        }

        public override void OnReceive(IChannelHandlerContext context, string str)
        {
            Collection.Get(LoggerId.Message)!.Information(str);

            var msg = Unpooled.Buffer();
            msg.WriteString($"OnReceive:{context.Channel.Id}", System.Text.Encoding.UTF8);
            context.WriteAndFlushAsync(msg);
        }
    }
}
