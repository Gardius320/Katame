using KatameApi.DTOs.Auth;

namespace KatameApi.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
    Task<AuthResponseDto> RefreshAsync(RefreshRequestDto request);
}
