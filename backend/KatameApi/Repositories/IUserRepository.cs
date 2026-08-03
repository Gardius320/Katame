using KatameApi.Models;

namespace KatameApi.Repositories;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByRefreshTokenAsync(string refreshToken);
    Task<bool> ExistsByUsernameAsync(string username);
    Task AddAsync(User user);
    Task SaveChangesAsync();
}
