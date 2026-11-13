using System.Security.Claims;
using ChessBackend.Modules.Multiplayer.DTOs;
using ChessBackend.Modules.Multiplayer.Services;
using ChessBackend.Shared.Contracts.Multiplayer;
using Microsoft.AspNetCore.SignalR;

namespace ChessBackend.Modules.Multiplayer.Hubs;

public class ChessHub : Hub
{
    private readonly RoomManager _roomManager;
    private readonly IUserConnectionTracker _connectionMapping;

    public ChessHub(RoomManager roomManager, IUserConnectionTracker connectionMapping)
    {
        _roomManager = roomManager;
        _connectionMapping = connectionMapping;
    }

    public override async Task OnConnectedAsync()
    {
        string? userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? Context.User?.FindFirstValue("sub")
                      ?? Context.GetHttpContext()?.Request.Query["userId"].ToString();

        if (!string.IsNullOrWhiteSpace(userId))
        {
            if (_connectionMapping.TryGetConnection(userId, out string? oldConnectionId) &&
                !string.IsNullOrEmpty(oldConnectionId) &&
                oldConnectionId != Context.ConnectionId)
            {
                await Clients.Client(oldConnectionId).SendAsync("ForceLogout", "Your account was accessed from another device.");
            }

            _connectionMapping.AddOrUpdate(userId, Context.ConnectionId);
        }

        await base.OnConnectedAsync();
    }

    public async Task<string> CreateRoom()
    {
        string roomCode = _roomManager.CreateRoom(Context.ConnectionId);

        await Groups.AddToGroupAsync(Context.ConnectionId, roomCode);

        return roomCode;
    }

    public async Task<bool> JoinRoom(string roomCode)
    {
        bool joined = _roomManager.JoinRoom(roomCode, Context.ConnectionId);

        if (joined)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomCode);

            await Clients.Group(roomCode).SendAsync("GameStarted");
            return true;
        }

        return false;
    }

    public async Task SendMove(ChessMoveDto move)
    {
        Console.WriteLine($"[MOVE RECEIVED] Room: {move.RoomCode} | From: {move.From} -> To: {move.To}");

        await Clients.OthersInGroup(move.RoomCode).SendAsync("ReceiveMove", move);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _connectionMapping.RemoveByConnectionId(Context.ConnectionId);

        _roomManager.RemovePlayer(Context.ConnectionId, out string? roomCode);

        if (!string.IsNullOrEmpty(roomCode))
        {
            await Clients.Group(roomCode).SendAsync("PlayerDisconnected");
        }

        await base.OnDisconnectedAsync(exception);
    }
}
