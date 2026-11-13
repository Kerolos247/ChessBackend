using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ChessBackend.Modules.Matches.Data;
using ChessBackend.Modules.Matches.DTOs;
using ChessBackend.Modules.Matches.Entities;
using ChessBackend.Shared.Contracts.Matches;

namespace ChessBackend.Modules.Matches.Services;

public class MatchService : IMatchService, IMatchModuleContract
{
    private readonly MatchesDbContext _dbContext;

    public MatchService(MatchesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<MatchResponseDto> RecordMatchAsync(Guid userId, CreateMatchRequestDto request)
    {
        var match = new Match
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Result = request.Result,
            Mode = request.Mode,
            PlayedAtUtc = DateTime.UtcNow
        };

        _dbContext.Matches.Add(match);
        await _dbContext.SaveChangesAsync();

        return new MatchResponseDto(match.Id, match.Result, match.Mode, match.PlayedAtUtc);
    }

    public async Task<List<MatchResponseDto>> GetUserMatchesAsync(Guid userId)
    {
        return await _dbContext.Matches
            .AsNoTracking()
            .Where(m => m.UserId == userId)
            .OrderByDescending(m => m.PlayedAtUtc)
            .Select(m => new MatchResponseDto(m.Id, m.Result, m.Mode, m.PlayedAtUtc))
            .ToListAsync();
    }

    public async Task<UserMatchStatsDto> GetUserStatsAsync(Guid userId)
    {
        var matches = await _dbContext.Matches
            .AsNoTracking()
            .Where(m => m.UserId == userId)
            .GroupBy(m => m.Result)
            .Select(g => new { Result = g.Key, Count = g.Count() })
            .ToListAsync();

        int wins = matches.FirstOrDefault(x => x.Result == MatchResult.Win)?.Count ?? 0;
        int losses = matches.FirstOrDefault(x => x.Result == MatchResult.Loss)?.Count ?? 0;
        int draws = matches.FirstOrDefault(x => x.Result == MatchResult.Draw)?.Count ?? 0;

        return new UserMatchStatsDto(wins + losses + draws, wins, losses, draws);
    }

    async Task<MatchContractDto> IMatchModuleContract.RecordMatchAsync(Guid userId, string result, string mode)
    {
        var parsedResult = Enum.TryParse<MatchResult>(result, true, out var r) ? r : MatchResult.Loss;
        var parsedMode = Enum.TryParse<MatchMode>(mode, true, out var m) ? m : MatchMode.OfflineWithFriend;
        var dto = await RecordMatchAsync(userId, new CreateMatchRequestDto(parsedResult, parsedMode));
        return new MatchContractDto(dto.Id, dto.Result.ToString(), dto.Mode.ToString(), dto.PlayedAtUtc);
    }

    async Task<List<MatchContractDto>> IMatchModuleContract.GetUserMatchesAsync(Guid userId)
    {
        var list = await GetUserMatchesAsync(userId);
        return list.Select(x => new MatchContractDto(x.Id, x.Result.ToString(), x.Mode.ToString(), x.PlayedAtUtc)).ToList();
    }

    async Task<UserMatchStatsContractDto> IMatchModuleContract.GetUserStatsAsync(Guid userId)
    {
        var stats = await GetUserStatsAsync(userId);
        return new UserMatchStatsContractDto(stats.TotalMatches, stats.Wins, stats.Losses, stats.Draws);
    }
}
