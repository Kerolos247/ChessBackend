using System.Threading.Tasks;
using ChessBackend.Modules.Auth.DTOs;

namespace ChessBackend.Modules.Auth.Services;

public interface IAuthService
{
    Task<AuthResponseDto> SignupAsync(SignupRequestDto request);
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
}
