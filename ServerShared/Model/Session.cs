using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Protocols.Common;
using Protocols.Response;
using ServerShared.Util;
using System.Threading.Tasks;

namespace ServerShared.Model
{
    public partial class Session
    {
        public string Name { get; set; }

        public Vector3 Position { get; set;}

        public Room Room { get; set; }

        public int? PlayerIndex { get; set; }

        private IChannelHandlerContext ChannelContext { get; }

        public Session(IChannelHandlerContext context)
        {
            this.ChannelContext = context;
        }

        public string GetSessionId()
        {
            return ChannelContext.GetHashCode().ToString();
        }

        private async Task Send(IByteBuffer byteBuffer)
        {
            await ChannelContext.WriteAndFlushAsync(byteBuffer);
        }

        public void Send<T>(T t) where T : Header
        {
            _ = Send(t.ToByteBuffer());
        }
    }
}
