using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ChessBackend.Modules.Auth.DTOs;
using ChessBackend.Modules.Auth.Services;

namespace ChessBackend.Modules.Auth.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("signup")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> Signup([FromBody] SignupRequestDto request)
    {
        var response = await _authService.SignupAsync(request);
        if (!response.Succeeded)
            return BadRequest(response);

        return Ok(response);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto request)
    {
        var response = await _authService.LoginAsync(request);
        if (!response.Succeeded)
            return Unauthorized(response);

        return Ok(response);
    }
}
