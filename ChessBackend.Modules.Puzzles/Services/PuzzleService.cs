using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ChessBackend.Modules.Puzzles.Data;
using ChessBackend.Modules.Puzzles.DTOs;
using ChessBackend.Modules.Puzzles.Entities;
using ChessBackend.Shared.Contracts.Puzzles;

namespace ChessBackend.Modules.Puzzles.Services;

public class PuzzleService : IPuzzleService, IPuzzleModuleContract
{
    private readonly PuzzlesDbContext _dbContext;

    public PuzzleService(PuzzlesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UnsolvedPuzzleResponseDto?> GetRandomUnsolvedPuzzleAsync(Guid userId)
    {
        return await _dbContext.Puzzles
            .AsNoTracking()
            .Where(p => !_dbContext.UserPuzzlesSolved
                .Any(ups => ups.UserId == userId && ups.PuzzleId == p.Id))
            .OrderBy(x => Guid.NewGuid())
            .Select(p => new UnsolvedPuzzleResponseDto(
                p.Id,
                p.FenBoardState,
                p.SolutionMoves))
            .FirstOrDefaultAsync();
    }

    public async Task<List<SolvedPuzzleItemDto>> GetSolvedPuzzlesAsync(Guid userId)
    {
        return await _dbContext.UserPuzzlesSolved
            .AsNoTracking()
            .Where(ups => ups.UserId == userId)
            .OrderByDescending(ups => ups.SolvedAtUtc)
            .Select(ups => new SolvedPuzzleItemDto(
                ups.PuzzleId,
                ups.Puzzle.FenBoardState,
                ups.Puzzle.SolutionMoves,
                ups.SolvedAtUtc))
            .ToListAsync();
    }

    public async Task<List<PuzzleResponseDto>> GetAllPuzzlesAsync(Guid? currentUserId)
    {
        var solvedPuzzleIds = currentUserId.HasValue
            ? await _dbContext.UserPuzzlesSolved
                .AsNoTracking()
                .Where(ups => ups.UserId == currentUserId.Value)
                .Select(ups => ups.PuzzleId)
                .ToHashSetAsync()
            : new HashSet<Guid>();

        var puzzles = await _dbContext.Puzzles
            .AsNoTracking()
            .Take(100)
            .ToListAsync();

        return puzzles
            .Select(p => new PuzzleResponseDto(
                p.Id,
                p.FenBoardState,
                p.SolutionMoves,
                solvedPuzzleIds.Contains(p.Id)))
            .ToList();
    }

    public async Task<PuzzleResponseDto?> GetPuzzleByIdAsync(Guid puzzleId, Guid? currentUserId)
    {
        var puzzle = await _dbContext.Puzzles
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == puzzleId);

        if (puzzle == null)
            return null;

        bool isSolved = currentUserId.HasValue && await _dbContext.UserPuzzlesSolved
            .AsNoTracking()
            .AnyAsync(ups => ups.UserId == currentUserId.Value && ups.PuzzleId == puzzleId);

        return new PuzzleResponseDto(puzzle.Id, puzzle.FenBoardState, puzzle.SolutionMoves, isSolved);
    }

    public async Task<PuzzleResponseDto> CreatePuzzleAsync(CreatePuzzleRequestDto request)
    {
        var puzzle = new Puzzle
        {
            Id = Guid.NewGuid(),
            FenBoardState = request.FenBoardState,
            SolutionMoves = request.SolutionMoves
        };

        _dbContext.Puzzles.Add(puzzle);
        await _dbContext.SaveChangesAsync();

        return new PuzzleResponseDto(puzzle.Id, puzzle.FenBoardState, puzzle.SolutionMoves, false);
    }

    public async Task<bool> MarkPuzzleAsSolvedAsync(Guid userId, Guid puzzleId)
    {
        var puzzleExists = await _dbContext.Puzzles.AnyAsync(p => p.Id == puzzleId);
        if (!puzzleExists)
            return false;

        var alreadySolved = await _dbContext.UserPuzzlesSolved
            .AnyAsync(ups => ups.UserId == userId && ups.PuzzleId == puzzleId);

        if (!alreadySolved)
        {
            _dbContext.UserPuzzlesSolved.Add(new UserPuzzleSolved
            {
                UserId = userId,
                PuzzleId = puzzleId,
                SolvedAtUtc = DateTime.UtcNow
            });
            await _dbContext.SaveChangesAsync();
        }

        return true;
    }

    public async Task<UserPuzzleStatsDto> GetUserPuzzleStatsAsync(Guid userId)
    {
        var solvedCount = await _dbContext.UserPuzzlesSolved
            .AsNoTracking()
            .CountAsync(ups => ups.UserId == userId);

        var totalCount = await _dbContext.Puzzles
            .AsNoTracking()
            .CountAsync();

        return new UserPuzzleStatsDto(solvedCount, totalCount);
    }

    async Task<UnsolvedPuzzleContractDto?> IPuzzleModuleContract.GetRandomUnsolvedPuzzleAsync(Guid userId)
    {
        var dto = await GetRandomUnsolvedPuzzleAsync(userId);
        return dto == null ? null : new UnsolvedPuzzleContractDto(dto.PuzzleId, dto.FenBoardState, dto.SolutionMoves);
    }

    async Task<List<SolvedPuzzleContractDto>> IPuzzleModuleContract.GetSolvedPuzzlesAsync(Guid userId)
    {
        var list = await GetSolvedPuzzlesAsync(userId);
        return list.Select(x => new SolvedPuzzleContractDto(x.PuzzleId, x.FenBoardState, x.SolutionMoves, x.SolvedAtUtc)).ToList();
    }
}
