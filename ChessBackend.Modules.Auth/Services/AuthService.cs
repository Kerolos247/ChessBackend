using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ChessBackend.Modules.Auth.DTOs;
using ChessBackend.Modules.Auth.Entities;
using ChessBackend.Shared.Contracts.Auth;

namespace ChessBackend.Modules.Auth.Services;

public class AuthService : IAuthService, IAuthModuleContract
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IConfiguration _configuration;

    public AuthService(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
    }

    public async Task<AuthResponseDto> SignupAsync(SignupRequestDto request)
    {
        var existingByEmail = await _userManager.FindByEmailAsync(request.Email);
        if (existingByEmail != null)
            return new AuthResponseDto(false, ErrorMessage: "Email is already in use.");

        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            UserName = request.Username,
            Email = request.Email
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            return new AuthResponseDto(false, ErrorMessage: errors);
        }

        return GenerateJwtResponse(user);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = request.UsernameOrEmail.Contains('@')
            ? await _userManager.FindByEmailAsync(request.UsernameOrEmail)
            : await _userManager.FindByNameAsync(request.UsernameOrEmail);

        if (user == null)
            return new AuthResponseDto(false, ErrorMessage: "Invalid credentials.");

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);
        if (!result.Succeeded)
            return new AuthResponseDto(false, ErrorMessage: "Invalid credentials.");

        return GenerateJwtResponse(user);
    }

    async Task<AuthResult> IAuthModuleContract.SignupAsync(SignupCommand command)
    {
        var dto = await SignupAsync(new SignupRequestDto(command.Username, command.Email, command.Password));
        return new AuthResult(dto.Succeeded, dto.Token, dto.ExpiresAtUtc, dto.UserId, dto.Username, dto.ErrorMessage);
    }

    async Task<AuthResult> IAuthModuleContract.LoginAsync(LoginCommand command)
    {
        var dto = await LoginAsync(new LoginRequestDto(command.UsernameOrEmail, command.Password));
        return new AuthResult(dto.Succeeded, dto.Token, dto.ExpiresAtUtc, dto.UserId, dto.Username, dto.ErrorMessage);
    }

    public async Task<bool> UserExistsAsync(Guid userId)
    {
        return await _userManager.Users.AsNoTracking().AnyAsync(u => u.Id == userId);
    }

    private AuthResponseDto GenerateJwtResponse(AppUser user)
    {
        var jwtSection = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSection["SecretKey"]
            ?? throw new InvalidOperationException("JwtSettings:SecretKey is missing in configuration.");
        var issuer = jwtSection["Issuer"] ?? "ChessBackend";
        var audience = jwtSection["Audience"] ?? "ChessWpfClient";
        var expiryMinutes = int.TryParse(jwtSection["ExpiryMinutes"], out var mins) ? mins : 1440;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(expiryMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.UserName ?? string.Empty),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return new AuthResponseDto(
            Succeeded: true,
            Token: tokenString,
            ExpiresAtUtc: expiresAtUtc,
            UserId: user.Id,
            Username: user.UserName);
    }
}
