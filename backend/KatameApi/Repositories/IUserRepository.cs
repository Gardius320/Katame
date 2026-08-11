using KatameApi.Models;

namespace KatameApi.Repositories;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByRefreshTokenAsync(string refreshToken);
    Task<User?> GetByPasswordResetTokenAsync(string token);
    Task<bool> ExistsByUsernameAsync(string username, int? excludeId = null);
    Task<List<User>> GetAllAsync();
    Task<User?> GetByIdAsync(int id);
    Task<bool> ExistsByEmailAsync(string email, int? excludeId = null);
    Task<bool> ExistsByDocumentIdAsync(string documentId, int? excludeId = null);
    Task<int> CountAdminsAsync();
    Task AddAsync(User user);
    void Remove(User user);
    Task SaveChangesAsync();
}
