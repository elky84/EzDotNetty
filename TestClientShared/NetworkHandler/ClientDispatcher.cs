using DotNetty.Transport.Channels;
using System;
using TestClientShared.Util;

namespace TestClientShared.NetworkHandler
{
    public abstract class ClientDispatcher
    {
        protected int UserIndex { get; set; }

        protected Random Random { get; set; } = new Random();

        protected ClientDispatcher()
        {
            EzDotNetty.Handler.Client.NetworkHandler.Subscribe<Protocols.Response.Login>(OnLogin);
            EzDotNetty.Handler.Client.NetworkHandler.Subscribe<Protocols.Response.Logout>(OnLogout);

            EzDotNetty.Handler.Client.NetworkHandler.Subscribe<Protocols.Response.Enter>(OnEnter);
            EzDotNetty.Handler.Client.NetworkHandler.Subscribe<Protocols.Response.Leave>(OnLeave);
            EzDotNetty.Handler.Client.NetworkHandler.Subscribe<Protocols.Response.Move>(OnMove);

            EzDotNetty.Handler.Client.NetworkHandler.Subscribe<Protocols.Response.Error>(OnError);
        }

        public virtual void Release()
        {
            EzDotNetty.Handler.Client.NetworkHandler.Unsubscribe<Protocols.Response.Login>(OnLogin);
            EzDotNetty.Handler.Client.NetworkHandler.Unsubscribe<Protocols.Response.Error>(OnError);
            EzDotNetty.Handler.Client.NetworkHandler.Unsubscribe<Protocols.Response.Enter>(OnEnter);
            EzDotNetty.Handler.Client.NetworkHandler.Unsubscribe<Protocols.Response.Leave>(OnLeave);
            EzDotNetty.Handler.Client.NetworkHandler.Unsubscribe<Protocols.Response.Move>(OnMove);
            EzDotNetty.Handler.Client.NetworkHandler.Unsubscribe<Protocols.Response.Error>(OnError);
        }

        protected abstract void OnLogin(Protocols.Response.Login login);
        protected abstract void OnLogout(Protocols.Response.Logout logout);

        protected abstract void OnEnter(Protocols.Response.Enter enter);
        protected abstract void OnLeave(Protocols.Response.Leave leave);
        protected abstract void OnMove(Protocols.Response.Move move);
        protected abstract void OnError(Protocols.Response.Error error);
    }
}
