using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ChessBackend.Modules.Puzzles.DTOs;
using ChessBackend.Modules.Puzzles.Services;

namespace ChessBackend.Modules.Puzzles.Controllers;

[ApiController]
[Route("api/puzzles")]
[Authorize]
public class PuzzleController : ControllerBase
{
    private readonly IPuzzleService _puzzleService;

    public PuzzleController(IPuzzleService puzzleService)
    {
        _puzzleService = puzzleService;
    }

    [HttpGet("random-unsolved")]
    public async Task<ActionResult<UnsolvedPuzzleResponseDto>> GetRandomUnsolvedPuzzle()
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized();

        var puzzle = await _puzzleService.GetRandomUnsolvedPuzzleAsync(userId.Value);
        if (puzzle == null)
            return NotFound(new { Message = "No unsolved puzzles remaining for this user." });

        return Ok(puzzle);
    }

    [HttpGet("solved")]
    public async Task<ActionResult<List<SolvedPuzzleItemDto>>> GetSolvedPuzzles()
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized();

        var solvedPuzzles = await _puzzleService.GetSolvedPuzzlesAsync(userId.Value);
        return Ok(solvedPuzzles);
    }

    [HttpPost("{puzzleId:guid}/solve")]
    public async Task<IActionResult> MarkPuzzleAsSolved(Guid puzzleId)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized();

        var success = await _puzzleService.MarkPuzzleAsSolvedAsync(userId.Value, puzzleId);
        if (!success)
            return NotFound(new { Message = "Puzzle not found." });

        return Ok(new { Message = "Puzzle marked as solved successfully." });
    }

    private Guid? GetCurrentUserId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        return Guid.TryParse(claim, out var id) ? id : null;
    }
}
