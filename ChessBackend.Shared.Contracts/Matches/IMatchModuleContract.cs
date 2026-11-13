using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChessBackend.Shared.Contracts.Matches;

public record MatchContractDto(
    Guid Id,
    string Result,
    string Mode,
    DateTime PlayedAtUtc
);

public record UserMatchStatsContractDto(
    int TotalMatches,
    int Wins,
    int Losses,
    int Draws
);

public interface IMatchModuleContract
{
    Task<MatchContractDto> RecordMatchAsync(Guid userId, string result, string mode);
    Task<List<MatchContractDto>> GetUserMatchesAsync(Guid userId);
    Task<UserMatchStatsContractDto> GetUserStatsAsync(Guid userId);
}
