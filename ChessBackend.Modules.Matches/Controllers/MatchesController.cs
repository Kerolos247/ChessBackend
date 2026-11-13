using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ChessBackend.Modules.Matches.DTOs;
using ChessBackend.Modules.Matches.Services;

namespace ChessBackend.Modules.Matches.Controllers;

[ApiController]
[Route("api/matches")]
[Authorize]
public class MatchesController : ControllerBase
{
    private readonly IMatchService _matchService;

    public MatchesController(IMatchService matchService)
    {
        _matchService = matchService;
    }

    [HttpPost]
    public async Task<ActionResult<MatchResponseDto>> RecordMatch([FromBody] CreateMatchRequestDto request)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized();

        var result = await _matchService.RecordMatchAsync(userId.Value, request);
        return Ok(result);
    }

    [HttpGet("history")]
    [HttpGet("my-history")]
    public async Task<ActionResult<List<MatchResponseDto>>> GetMyMatches()
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized();

        var matches = await _matchService.GetUserMatchesAsync(userId.Value);
        return Ok(matches);
    }

    [HttpGet("my-stats")]
    public async Task<ActionResult<UserMatchStatsDto>> GetMyStats()
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized();

        var stats = await _matchService.GetUserStatsAsync(userId.Value);
        return Ok(stats);
    }

    private Guid? GetCurrentUserId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        return Guid.TryParse(claim, out var id) ? id : null;
    }
}
