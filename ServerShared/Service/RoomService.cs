using Serilog;
using ServerShared.Model;
using System.Collections.Generic;

namespace ServerShared.Service
{
    public partial class RoomService
    {

        private readonly Dictionary<int, Room> Rooms = new();

        public Room New(int roomId)
        {
            var room = new Room
            {
                Id = roomId,
            };

            Rooms.Add(room.Id, room);
            return room;
        }

        public void Remove(int id)
        {
            Rooms.Remove(id);
        }

        public Room Get(int id)
        {
            return Rooms.TryGetValue(id, out var room) ? room : null;
        }

        public void Enter(Session session, Protocols.Request.Enter enter)
        {
            var room = Get(enter.RoomId);
            if(room == null)
            {
                room = New(enter.RoomId);
            }

            room.Enter(session, enter);
        }

        public void Leave(Session session, Protocols.Request.Leave leave)
        {
            var room = session.Room;
            if (room == null)
            {
                Log.Error($"Leave() <Desc:Not found room> <Session:{session}>");
                return;
            }

            room.Leave(session, leave);

            if(room.IsEmpty())
            {
                Remove(room.Id);
            }
        }

        public void Move(Session session, Protocols.Request.Move move)
        {
            var room = session.Room;
            if (room == null)
            {
                Log.Error($"Leave() <Desc:Not found room> <Session:{session}>");
                return;
            }

            room.Move(session, move);
        }

    }
}
