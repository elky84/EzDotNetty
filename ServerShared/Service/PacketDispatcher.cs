using LightInject;
using ServerShared.Model;
using EzDotNetty.Logging;

namespace ServerShared.Service
{
    public class PacketDispatcher
    {
        [Inject]
        public SessionService SessionService { get; set; }

        [Inject]
        public RoomService RoomService { get; set; }

        public bool Login(Session session, Protocols.Request.Login login)
        {
            if (null != SessionService.Get(login.Name))
            {
                session.Send(new Protocols.Response.Login { Result = Protocols.Code.Result.DuplicateNickname });
                return false;
            }

            if (string.IsNullOrEmpty(login.Name))
            {
                session.Send(new Protocols.Response.Login { Result = Protocols.Code.Result.InvalidNickname });
                return false;
            }

            Collection.Get(LoggerId.Message)!.Information($"Login() <Name:{login.Name}>");

            SessionService.SetSessionName(session, login.Name);

            session.Send(new Protocols.Response.Login { Name = login.Name });
            return true;
        }

        public bool Logout(Session session, Protocols.Request.Logout logout)
        {
            if (string.IsNullOrEmpty(session.Name))
            {
                session.Send(new Protocols.Response.Login { Result = Protocols.Code.Result.NotHaveNickname });
                return false;
            }

            if(session.Room != null)
            {
                session.Room.Leave(session, new Protocols.Request.Leave { });
            }

            Collection.Get(LoggerId.Message)!.Information($"Logout() <Name:{session.Name}>");
            SessionService.UnsetSessionName(session);

            session.Send(new Protocols.Response.Logout { });
            return true;
        }

        public bool Enter(Session session, Protocols.Request.Enter enter)
        {
            RoomService.Enter(session, enter);
            return true;
        }

        public bool Leave(Session session, Protocols.Request.Leave leave)
        {
            RoomService.Leave(session, leave);
            return true;
        }

        public bool Move(Session session, Protocols.Request.Move move)
        {
            RoomService.Move(session, move);
            return true;
        }
    }
}
