using DotNetty.Transport.Channels;
using System;
using TestClientShared.Util;

namespace TestClientShared.NetworkHandler
{
    public abstract class ClientDispatcher
    {
        protected int UserIndex { get; set; }

        protected Random Random { get; set; } = new Random();

        public ClientDispatcher()
        {
            ClientHandler.Subscribe<Protocols.Response.Login>(OnLogin);
            ClientHandler.Subscribe<Protocols.Response.Logout>(OnLogout);

            ClientHandler.Subscribe<Protocols.Response.Enter>(OnEnter);
            ClientHandler.Subscribe<Protocols.Response.Leave>(OnLeave);
            ClientHandler.Subscribe<Protocols.Response.Move>(OnMove);

            ClientHandler.Subscribe<Protocols.Response.Error>(OnError);
        }

        public virtual void Release()
        {
            ClientHandler.Unsubscribe<Protocols.Response.Login>(OnLogin);
            ClientHandler.Unsubscribe<Protocols.Response.Error>(OnError);
            ClientHandler.Unsubscribe<Protocols.Response.Enter>(OnEnter);
            ClientHandler.Unsubscribe<Protocols.Response.Leave>(OnLeave);
            ClientHandler.Unsubscribe<Protocols.Response.Move>(OnMove);
            ClientHandler.Unsubscribe<Protocols.Response.Error>(OnError);
        }

        protected abstract void OnLogin(Protocols.Response.Login login);
        protected abstract void OnLogout(Protocols.Response.Logout logout);

        protected abstract void OnEnter(Protocols.Response.Enter enter);
        protected abstract void OnLeave(Protocols.Response.Leave leave);
        protected abstract void OnMove(Protocols.Response.Move move);
        protected abstract void OnError(Protocols.Response.Error error);
    }
}
