using System;
using System.Threading.Tasks;

namespace ChessBackend.Shared.Contracts.Auth;

public record SignupCommand(string Username, string Email, string Password);

public record LoginCommand(string UsernameOrEmail, string Password);

public record AuthResult(
    bool Succeeded,
    string? Token = null,
    DateTime? ExpiresAtUtc = null,
    Guid? UserId = null,
    string? Username = null,
    string? ErrorMessage = null
);

public interface IAuthModuleContract
{
    Task<AuthResult> SignupAsync(SignupCommand command);
    Task<AuthResult> LoginAsync(LoginCommand command);
    Task<bool> UserExistsAsync(Guid userId);
}
