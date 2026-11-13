using System;
using ChessBackend.Modules.Matches.Entities;

namespace ChessBackend.Modules.Matches.DTOs;

public record CreateMatchRequestDto(
    MatchResult Result,
    MatchMode Mode
);

public record MatchResponseDto(
    Guid Id,
    MatchResult Result,
    MatchMode Mode,
    DateTime PlayedAtUtc
);

public record UserMatchStatsDto(
    int TotalMatches,
    int Wins,
    int Losses,
    int Draws
);
