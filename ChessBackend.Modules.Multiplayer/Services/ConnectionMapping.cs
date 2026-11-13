using System.Collections.Concurrent;
using ChessBackend.Shared.Contracts.Multiplayer;

namespace ChessBackend.Modules.Multiplayer.Services;

public class ConnectionMapping<T> where T : notnull
{
    private readonly ConcurrentDictionary<T, string> _userToConnection = new();
    private readonly ConcurrentDictionary<string, T> _connectionToUser = new();

    public bool TryGetConnection(T key, out string? connectionId)
    {
        return _userToConnection.TryGetValue(key, out connectionId);
    }

    public void AddOrUpdate(T key, string connectionId)
    {
        if (_userToConnection.TryGetValue(key, out var oldConnectionId) && oldConnectionId != connectionId)
        {
            _connectionToUser.TryRemove(oldConnectionId, out _);
        }

        _userToConnection[key] = connectionId;
        _connectionToUser[connectionId] = key;
    }

    public void RemoveByConnectionId(string connectionId)
    {
        if (_connectionToUser.TryRemove(connectionId, out var key))
        {
            _userToConnection.TryRemove(new KeyValuePair<T, string>(key, connectionId));
        }
    }
}

public class UserConnectionTracker : ConnectionMapping<string>, IUserConnectionTracker
{
}
