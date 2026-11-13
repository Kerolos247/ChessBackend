using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChessBackend.Shared.Contracts.Puzzles;

public record UnsolvedPuzzleContractDto(
    Guid PuzzleId,
    string FenBoardState,
    string SolutionMoves
);

public record SolvedPuzzleContractDto(
    Guid PuzzleId,
    string FenBoardState,
    string SolutionMoves,
    DateTime SolvedAtUtc
);

public interface IPuzzleModuleContract
{
    Task<UnsolvedPuzzleContractDto?> GetRandomUnsolvedPuzzleAsync(Guid userId);
    Task<List<SolvedPuzzleContractDto>> GetSolvedPuzzlesAsync(Guid userId);
    Task<bool> MarkPuzzleAsSolvedAsync(Guid userId, Guid puzzleId);
}
