using System;
using System.Collections.Generic;

namespace ChessBackend.Modules.Puzzles.Entities;

public class Puzzle
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FenBoardState { get; set; } = string.Empty;
    public string SolutionMoves { get; set; } = string.Empty;
    public ICollection<UserPuzzleSolved> UserPuzzlesSolved { get; set; } = new List<UserPuzzleSolved>();
}
