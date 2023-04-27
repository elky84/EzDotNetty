using Serilog;
using ServerShared.Model;
using System.Collections.Generic;

namespace ServerShared.Service
{
    public partial class RoomService
    {

        private readonly Dictionary<int, Room> Rooms = new();

        private Room New(int roomId)
        {
            var room = new Room
            {
                Id = roomId,
            };

            Rooms.Add(room.Id, room);
            return room;
        }

        private void Remove(int id)
        {
            Rooms.Remove(id);
        }

        private Room Get(int id)
        {
            return Rooms.TryGetValue(id, out var room) ? room : null;
        }

        public void Enter(Session session, Protocols.Request.Enter enter)
        {
            var room = Get(enter.RoomId) ?? New(enter.RoomId);

            room.Enter(session, enter);
        }

        public void Leave(Session session, Protocols.Request.Leave leave)
        {
            var room = session.Room;
            if (room == null)
            {
                Log.Error("Leave() <Desc:Not found room> <Session:{Session}>", session.ToString());
                return;
            }

            room.Leave(session, leave);

            if(room.IsEmpty())
            {
                Remove(room.Id);
            }
        }

        public static void Move(Session session, Protocols.Request.Move move)
        {
            var room = session.Room;
            if (room == null)
            {
                Log.Error("Leave() <Desc:Not found room> <Session:{Session}>", session.ToString());
                return;
            }

            room.Move(session, move);
        }

    }
}
