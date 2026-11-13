namespace ChessBackend.Shared.Contracts.Multiplayer;

public interface IUserConnectionTracker
{
    bool TryGetConnection(string userId, out string? connectionId);
    void AddOrUpdate(string userId, string connectionId);
    void RemoveByConnectionId(string connectionId);
}
