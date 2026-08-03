using KatameApi.Models;

namespace KatameApi.Services;

public interface ITokenService
{
    (string Token, DateTime Expiry) GenerateAccessToken(User user);
    string GenerateRefreshToken();
}
