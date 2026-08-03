using System.Net;
using KatameApi.DTOs.Auth;
using KatameApi.Middleware;
using KatameApi.Models;
using KatameApi.Repositories;

namespace KatameApi.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly JwtSettings _jwtSettings;

    public AuthService(IUserRepository userRepository, ITokenService tokenService, Microsoft.Extensions.Options.IOptions<JwtSettings> jwtSettings)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        if (await _userRepository.ExistsByUsernameAsync(request.Username))
        {
            throw new ApiException("Ese nombre de usuario ya está en uso.", HttpStatusCode.Conflict);
        }

        var user = new User
        {
            Username = request.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        return await IssueTokensAsync(user);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await _userRepository.GetByUsernameAsync(request.Username);

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            throw new ApiException("Usuario o contraseña incorrectos.", HttpStatusCode.Unauthorized);
        }

        return await IssueTokensAsync(user);
    }

    public async Task<AuthResponseDto> RefreshAsync(RefreshRequestDto request)
    {
        var user = await _userRepository.GetByRefreshTokenAsync(request.RefreshToken);

        if (user is null || user.RefreshTokenExpiry is null || user.RefreshTokenExpiry < DateTime.UtcNow)
        {
            throw new ApiException("La sesión expiró. Inicia sesión de nuevo.", HttpStatusCode.Unauthorized);
        }

        return await IssueTokensAsync(user);
    }

    private async Task<AuthResponseDto> IssueTokensAsync(User user)
    {
        var (accessToken, expiry) = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays);
        await _userRepository.SaveChangesAsync();

        return new AuthResponseDto
        {
            Username = user.Username,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiry = expiry
        };
    }
}
