using KatameApi.Models;
using KatameApi.Repositories;

namespace KatameApi.Tests.Services;

public class FakeUserRepository : IUserRepository
{
    private readonly List<User> _users = new();
    private int _nextId = 1;

    public Task<User?> GetByUsernameAsync(string username) =>
        Task.FromResult(_users.FirstOrDefault(u => u.Username == username));

    public Task<User?> GetByEmailAsync(string email) =>
        Task.FromResult(_users.FirstOrDefault(u => u.Email == email));

    public Task<User?> GetByRefreshTokenAsync(string refreshToken) =>
        Task.FromResult(_users.FirstOrDefault(u => u.RefreshToken == refreshToken));

    public Task<User?> GetByPasswordResetTokenAsync(string token) =>
        Task.FromResult(_users.FirstOrDefault(u => u.PasswordResetToken == token));

    public Task<bool> ExistsByUsernameAsync(string username, int? excludeId = null) =>
        Task.FromResult(_users.Any(u => u.Username == username && u.Id != excludeId));

    public Task<List<User>> GetAllAsync() =>
        Task.FromResult(_users.OrderBy(u => u.Username).ToList());

    public Task<User?> GetByIdAsync(int id) =>
        Task.FromResult(_users.FirstOrDefault(u => u.Id == id));

    public Task<bool> ExistsByEmailAsync(string email, int? excludeId = null) =>
        Task.FromResult(_users.Any(u => u.Email == email && u.Id != excludeId));

    public Task<bool> ExistsByDocumentIdAsync(string documentId, int? excludeId = null) =>
        Task.FromResult(_users.Any(u => u.DocumentId == documentId && u.Id != excludeId));

    public Task<int> CountAdminsAsync() =>
        Task.FromResult(_users.Count(u => u.IsAdmin));

    public Task AddAsync(User user)
    {
        user.Id = _nextId++;
        _users.Add(user);
        return Task.CompletedTask;
    }

    public void Remove(User user) => _users.Remove(user);

    public Task SaveChangesAsync() => Task.CompletedTask;
}
