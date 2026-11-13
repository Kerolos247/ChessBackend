using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChessBackend.Modules.Matches.DTOs;

namespace ChessBackend.Modules.Matches.Services;

public interface IMatchService
{
    Task<MatchResponseDto> RecordMatchAsync(Guid userId, CreateMatchRequestDto request);
    Task<List<MatchResponseDto>> GetUserMatchesAsync(Guid userId);
    Task<UserMatchStatsDto> GetUserStatsAsync(Guid userId);
}
