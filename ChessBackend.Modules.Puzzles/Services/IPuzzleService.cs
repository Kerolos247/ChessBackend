using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChessBackend.Modules.Puzzles.DTOs;

namespace ChessBackend.Modules.Puzzles.Services;

public interface IPuzzleService
{
    Task<UnsolvedPuzzleResponseDto?> GetRandomUnsolvedPuzzleAsync(Guid userId);
    Task<List<SolvedPuzzleItemDto>> GetSolvedPuzzlesAsync(Guid userId);
    Task<List<PuzzleResponseDto>> GetAllPuzzlesAsync(Guid? currentUserId);
    Task<PuzzleResponseDto?> GetPuzzleByIdAsync(Guid puzzleId, Guid? currentUserId);
    Task<PuzzleResponseDto> CreatePuzzleAsync(CreatePuzzleRequestDto request);
    Task<bool> MarkPuzzleAsSolvedAsync(Guid userId, Guid puzzleId);
    Task<UserPuzzleStatsDto> GetUserPuzzleStatsAsync(Guid userId);
}
