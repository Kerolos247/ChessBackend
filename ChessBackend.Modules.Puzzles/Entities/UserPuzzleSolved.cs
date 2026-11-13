using System;

namespace ChessBackend.Modules.Puzzles.Entities;

public class UserPuzzleSolved
{
    public Guid UserId { get; set; }
    public Guid PuzzleId { get; set; }
    public Puzzle Puzzle { get; set; } = null!;
    public DateTime SolvedAtUtc { get; set; } = DateTime.UtcNow;
}
