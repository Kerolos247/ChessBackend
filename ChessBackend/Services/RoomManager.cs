using ChessBackend.Models;
using System.Collections.Concurrent;

namespace ChessBackend.Services
{
    public class RoomManager
    {
        private readonly ConcurrentDictionary<string, ChessRoom> _rooms = new();

        // 1. إنشاء غرفة وتوليد كود عشوائي من 6 أرقام/حروف
        public string CreateRoom(string connectionId)
        {
            string roomCode;
            do
            {
                roomCode = Guid.NewGuid().ToString()[..6].ToUpper();
            } while (_rooms.ContainsKey(roomCode));

            var room = new ChessRoom
            {
                RoomCode = roomCode,
                WhitePlayerConnectionId = connectionId
            };

            _rooms[roomCode] = room;
            return roomCode;
        }

        // 2. انضمام لاعب تاني للغرفة
        public bool JoinRoom(string roomCode, string connectionId)
        {
            if (_rooms.TryGetValue(roomCode, out var room) && !room.IsFull)
            {
                room.BlackPlayerConnectionId = connectionId;
                return true;
            }
            return false;
        }

        // 3. حذف الغرفة عند خروج لاعب
        public void RemovePlayer(string connectionId, out string? roomCode)
        {
            roomCode = null;
            var room = _rooms.Values.FirstOrDefault(r =>
                r.WhitePlayerConnectionId == connectionId || r.BlackPlayerConnectionId == connectionId);

            if (room != null)
            {
                roomCode = room.RoomCode;
                _rooms.TryRemove(roomCode, out _);
            }
        }
    }
}