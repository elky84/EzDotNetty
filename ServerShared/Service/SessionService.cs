using DotNetty.Transport.Channels;
using Serilog;
using ServerShared.Model;
using System.Collections.Generic;

namespace ServerShared.Service
{
    public partial class SessionService
    {

        private readonly Dictionary<IChannelHandlerContext, Session> Sessions = new();

        private readonly Dictionary<string, Session> SessionsByName = new();

        public void Add(IChannelHandlerContext context)
        {
            Sessions.Add(context, new Session(context));
        }

        public void SetSessionName(Session session, string name)
        {
            session.Name = name;
            SessionsByName.Add(session.Name, session);
        }

        public void Remove(IChannelHandlerContext context)
        {
            var session = Get(context);
            if (null == session)
            {
                Log.Information($"Session Get Failed. <Context:{context}>");
                return;
            }

            if (session.Room != null)
            {
                session.Room.Leave(session, new Protocols.Request.Leave { });
            }

            UnsetSessionName(session);

            Sessions.Remove(context);
        }

        public void UnsetSessionName(Session session)
        {
            if (!string.IsNullOrEmpty(session.Name) && SessionsByName.ContainsKey(session.Name))
            {
                SessionsByName.Remove(session.Name);
                session.Name = string.Empty;
            }
        }

        public Session Get(IChannelHandlerContext context)
        {
            return Sessions.TryGetValue(context, out var session) ? session : null;
        }

        public Session Get(string name)
        {
            return SessionsByName.TryGetValue(name, out var session) ? session : null;
        }

        public void Broadcast(Protocols.Response.Header header)
        {
            foreach (var session in Sessions.Values)
            {
                session.Send(header);
            }
        }
    }
}
