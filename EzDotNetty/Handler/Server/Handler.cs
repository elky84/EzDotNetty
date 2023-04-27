using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Serilog;

namespace EzDotNetty.Handler.Server
{
    public abstract class NetworkHandler : ChannelHandlerAdapter
    {

        public override void ChannelActive(IChannelHandlerContext context)
        {
            base.ChannelActive(context);
            OnChannelActive(context);
        }

        public override void ChannelInactive(IChannelHandlerContext context)
        {
            base.ChannelInactive(context);
            OnChannelInactive(context);
        }

        public override void ChannelRead(IChannelHandlerContext context, object byteBuffer)
        {
            var buffer = byteBuffer as IByteBuffer;
            var id = buffer!.ReadInt();

            var bytes = new byte[buffer!.ReadableBytes];
            buffer.ReadBytes(bytes);

            OnReceive(context, id, bytes);
        }

        public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            Log.Logger.Error("<Exception:{ExceptionMessage}>", exception.Message);
            context.CloseAsync();
        }

        protected abstract void OnChannelActive(IChannelHandlerContext context);

        protected abstract void OnChannelInactive(IChannelHandlerContext context);

        protected abstract void OnReceive(IChannelHandlerContext context, int id, byte[] bytes);
    }
}