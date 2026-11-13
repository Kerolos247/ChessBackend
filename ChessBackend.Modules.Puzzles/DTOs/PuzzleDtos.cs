using System;
using System.ComponentModel.DataAnnotations;

namespace ChessBackend.Modules.Puzzles.DTOs;

public record UnsolvedPuzzleResponseDto(
    Guid PuzzleId,
    string FenBoardState,
    string SolutionMoves
);

public record SolvedPuzzleItemDto(
    Guid PuzzleId,
    string FenBoardState,
    string SolutionMoves,
    DateTime SolvedAtUtc
);

public record CreatePuzzleRequestDto(
    [Required] string FenBoardState,
    [Required] string SolutionMoves
);

public record PuzzleResponseDto(
    Guid Id,
    string FenBoardState,
    string SolutionMoves,
    bool IsSolvedByCurrentUser = false
);

public record UserPuzzleStatsDto(
    int TotalSolvedPuzzles,
    int TotalAvailablePuzzles
);
