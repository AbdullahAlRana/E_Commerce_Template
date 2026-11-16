using ECommerce.Application.DTOs.Auth;
using ECommerce.Application.Services.Interfaces;
using ECommerce.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace ECommerce.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _users;
    private readonly SignInManager<ApplicationUser> _signIn;
    private readonly JwtTokenService _jwt;
    private readonly IConfiguration _config;

    public AuthService(UserManager<ApplicationUser> users, SignInManager<ApplicationUser> signIn, JwtTokenService jwt, IConfiguration config)
    {
        _users = users; _signIn = signIn; _jwt = jwt; _config = config;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        var existing = await _users.FindByEmailAsync(request.Email);
        if (existing != null) throw new InvalidOperationException("Email already registered.");
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.Phone
        };
        var result = await _users.CreateAsync(user, request.Password);
        if (!result.Succeeded) throw new InvalidOperationException(string.Join("; ", result.Errors.Select(e => e.Description)));
        await _users.AddToRoleAsync(user, "Customer");
        return await IssueTokensAsync(user);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var user = await _users.FindByEmailAsync(request.Email);
        if (user == null || !user.IsActive) throw new UnauthorizedAccessException("Invalid credentials.");
        var result = await _signIn.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded) throw new UnauthorizedAccessException("Invalid credentials.");
        return await IssueTokensAsync(user);
    }

    public async Task<AuthResponse> RefreshAsync(string refreshToken, CancellationToken ct = default)
    {
        var user = _users.Users.FirstOrDefault(u => u.RefreshToken == refreshToken);
        if (user == null || user.RefreshTokenExpiry == null || user.RefreshTokenExpiry < DateTime.UtcNow)
            throw new UnauthorizedAccessException("Invalid refresh token.");
        return await IssueTokensAsync(user);
    }

    private async Task<AuthResponse> IssueTokensAsync(ApplicationUser user)
    {
        var roles = await _users.GetRolesAsync(user);
        var (access, expires) = _jwt.CreateAccessToken(user, roles);
        var refresh = _jwt.CreateRefreshToken();
        user.RefreshToken = refresh;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(double.Parse(_config["Jwt:RefreshTokenDays"] ?? "7"));
        await _users.UpdateAsync(user);
        return new AuthResponse
        {
            AccessToken = access,
            RefreshToken = refresh,
            ExpiresAt = expires,
            User = new UserInfo
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Phone = user.PhoneNumber,
                Roles = roles
            }
        };
    }
}