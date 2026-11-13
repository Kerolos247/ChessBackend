using System;

namespace ChessBackend.Modules.Matches.Entities;

public class Match
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public MatchResult Result { get; set; }
    public MatchMode Mode { get; set; }
    public DateTime PlayedAtUtc { get; set; } = DateTime.UtcNow;
    public Guid UserId { get; set; }
}
