using DotNetty.Transport.Channels;
using Serilog;
using ServerShared.NetworkHandler;
using ServerShared.Service;
using ServerShared.Worker;
using EzDotNetty.Handler.Server;

namespace TestServer.Handler
{
    public class TestServerHandler : NetworkHandler
    {
        public SessionService SessionService { get; set; }

        public ServerDispatcher ServerDispatcher { get; set; }

        protected override void OnChannelActive(IChannelHandlerContext context)
        {
            this.SessionService = ServerService.GetInstance<SessionService>();
            this.ServerDispatcher = ServerService.GetInstance<ServerDispatcher>();

            Log.Information($"OnChannelActive() <Context:{context}>");

            SessionService.Add(context);
        }

        protected override void OnChannelInactive(IChannelHandlerContext context)
        {
            Log.Information($"OnChannelInactive() <Context:{context}>");

            SessionService.Remove(context);
        }

        protected override void OnReceive(IChannelHandlerContext context, int id, byte[] bytes)
        {
            var session = SessionService.Get(context);
            if (null == session)
            {
                Log.Error($"Session Get Failed() <Context:{context}>");
                return;
            }

            var message = new Message
            {
                Id = (Protocols.Id.Request)id,
                Data = bytes,
                Session = session
            };

            ServerDispatcher.Push(message);
        }
    }
}