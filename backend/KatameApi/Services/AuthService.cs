using System.Net;
using System.Security.Cryptography;
using KatameApi.DTOs.Auth;
using KatameApi.Middleware;
using KatameApi.Models;
using KatameApi.Repositories;

namespace KatameApi.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;
    private readonly JwtSettings _jwtSettings;
    private readonly FrontendSettings _frontendSettings;

    public AuthService(
        IUserRepository userRepository,
        ITokenService tokenService,
        IEmailService emailService,
        Microsoft.Extensions.Options.IOptions<JwtSettings> jwtSettings,
        Microsoft.Extensions.Options.IOptions<FrontendSettings> frontendSettings)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _emailService = emailService;
        _jwtSettings = jwtSettings.Value;
        _frontendSettings = frontendSettings.Value;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        if (await _userRepository.ExistsByEmailAsync(request.Email))
        {
            throw new ApiException("Ese correo ya está en uso.", HttpStatusCode.Conflict);
        }

        if (await _userRepository.ExistsByDocumentIdAsync(request.DocumentId))
        {
            throw new ApiException("Esa cédula ya está registrada.", HttpStatusCode.Conflict);
        }

        var user = new User
        {
            // El formulario de registro no pide un nombre de usuario propio: se usa
            // el correo completo como Username. Como el correo ya se valida como
            // único más arriba (ExistsByEmailAsync), el índice único de Username
            // queda cubierto sin necesidad de generar ni desambiguar nada.
            Username = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            DocumentId = request.DocumentId,
            PhoneNumber = request.PhoneNumber,
            Email = request.Email,
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
            FirstName = user.FirstName,
            IsAdmin = user.IsAdmin,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiry = expiry
        };
    }

    public async Task ForgotPasswordAsync(ForgotPasswordRequestDto request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);

        // Si el correo no existe, no se lanza error ni se informa nada distinto:
        // devolver siempre la misma respuesta evita que alguien use este endpoint
        // para averiguar qué correos están registrados en Katame.
        if (user is null)
        {
            return;
        }

        var token = GenerateSecureToken();
        user.PasswordResetToken = token;
        user.PasswordResetTokenExpiry = DateTime.UtcNow.AddMinutes(30);
        await _userRepository.SaveChangesAsync();

        var resetLink = $"{_frontendSettings.BaseUrl}/reset-password?token={Uri.EscapeDataString(token)}";
        await _emailService.SendPasswordResetEmailAsync(user.Email, user.FirstName, resetLink);
    }

    public async Task ResetPasswordAsync(ResetPasswordRequestDto request)
    {
        var user = await _userRepository.GetByPasswordResetTokenAsync(request.Token);

        if (user is null || user.PasswordResetTokenExpiry is null || user.PasswordResetTokenExpiry < DateTime.UtcNow)
        {
            throw new ApiException("El enlace de recuperación no es válido o ya expiró.", HttpStatusCode.Unauthorized);
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        user.PasswordResetToken = null;
        user.PasswordResetTokenExpiry = null;
        // Se invalida cualquier sesión activa: si alguien más tenía acceso con la
        // contraseña vieja, este cambio lo saca y obliga a loguearse de nuevo.
        user.RefreshToken = null;
        user.RefreshTokenExpiry = null;
        await _userRepository.SaveChangesAsync();
    }

    public async Task LogoutAsync(RefreshRequestDto request)
    {
        var user = await _userRepository.GetByRefreshTokenAsync(request.RefreshToken);

        // Si el token ya no existe o ya expiro, no hay nada que revocar --
        // se responde igual sin error (mismo espiritu que ForgotPasswordAsync:
        // no filtrar informacion, y ademas hace que cerrar sesion sea
        // idempotente sin importar el estado del token).
        if (user is null)
        {
            return;
        }

        user.RefreshToken = null;
        user.RefreshTokenExpiry = null;
        await _userRepository.SaveChangesAsync();
    }

    private static string GenerateSecureToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(randomBytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');
    }
}
