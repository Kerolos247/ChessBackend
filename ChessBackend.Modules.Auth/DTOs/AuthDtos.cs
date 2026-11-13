using System;
using System.ComponentModel.DataAnnotations;

namespace ChessBackend.Modules.Auth.DTOs;

public record SignupRequestDto(
    [Required, MinLength(3), MaxLength(32)] string Username,
    [Required, EmailAddress] string Email,
    [Required, MinLength(6)] string Password
);

public record LoginRequestDto(
    [Required] string UsernameOrEmail,
    [Required] string Password
);

public record AuthResponseDto(
    bool Succeeded,
    string? Token = null,
    DateTime? ExpiresAtUtc = null,
    Guid? UserId = null,
    string? Username = null,
    string? ErrorMessage = null
);
