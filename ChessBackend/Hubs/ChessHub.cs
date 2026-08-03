using ChessBackend.Models;
using ChessBackend.Services;
using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;

namespace ChessBackend.Hubs
{
    public class ChessHub : Hub
    {
        private readonly RoomManager _roomManager;

        public ChessHub(RoomManager roomManager)
        {
            _roomManager = roomManager;
        }

        // 1. Host يطلب إنشاء غرفة
        public async Task<string> CreateRoom()
        {
            string roomCode = _roomManager.CreateRoom(Context.ConnectionId);

            // ربط الـ Connection بتجمع SignalR Group
            await Groups.AddToGroupAsync(Context.ConnectionId, roomCode);

            return roomCode; // يرجع الكود فوراً لـ CreateRoomPage
        }

        // 2. Guest بيبعت الكود للانضمام
        public async Task<bool> JoinRoom(string roomCode)
        {
            bool joined = _roomManager.JoinRoom(roomCode, Context.ConnectionId);

            if (joined)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, roomCode);

                // إبلاغ الطرفين (Host & Guest) داخل الغرفة أن المباراة بدأت
                await Clients.Group(roomCode).SendAsync("GameStarted");
                return true;
            }

            return false; // الغرفة مش موجودة أو مليانة
        }

        // 3. تمرير النقلة للخصم
        public async Task SendMove(ChessMoveDto move)
        {
            // أضف السطر ده عشان تشوف النقلة في الـ Console بتاعة الـ Backend
            Console.WriteLine($"[MOVE RECEIVED] Room: {move.RoomCode} | From: {move.From} -> To: {move.To}");

            await Clients.OthersInGroup(move.RoomCode).SendAsync("ReceiveMove", move);
        }

        // 4. التعامل مع فصل الاتصال (Disconnect)
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _roomManager.RemovePlayer(Context.ConnectionId, out string? roomCode);

            if (!string.IsNullOrEmpty(roomCode))
            {
                // إبلاغ الخصم إن التاني انسحب/فصل
                await Clients.Group(roomCode).SendAsync("PlayerDisconnected");
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
