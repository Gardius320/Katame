using Microsoft.EntityFrameworkCore;
using KatameApi.Data;
using KatameApi.Models;

namespace KatameApi.Repositories;

public class UserRepository : IUserRepository
{
    private readonly KatameDbContext _context;

    public UserRepository(KatameDbContext context)
    {
        _context = context;
    }

    public Task<User?> GetByUsernameAsync(string username) =>
        _context.Users.FirstOrDefaultAsync(u => u.Username == username);

    public Task<User?> GetByEmailAsync(string email) =>
        _context.Users.FirstOrDefaultAsync(u => u.Email == email);

    public Task<User?> GetByRefreshTokenAsync(string refreshToken) =>
        _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

    public Task<User?> GetByPasswordResetTokenAsync(string token) =>
        _context.Users.FirstOrDefaultAsync(u => u.PasswordResetToken == token);

    public Task<bool> ExistsByUsernameAsync(string username, int? excludeId = null) =>
        _context.Users.AnyAsync(u => u.Username == username && u.Id != excludeId);

    public Task<List<User>> GetAllAsync() =>
        _context.Users.OrderBy(u => u.Username).ToListAsync();

    public Task<User?> GetByIdAsync(int id) =>
        _context.Users.FirstOrDefaultAsync(u => u.Id == id);

    public Task<bool> ExistsByEmailAsync(string email, int? excludeId = null) =>
        _context.Users.AnyAsync(u => u.Email == email && u.Id != excludeId);

    public Task<bool> ExistsByDocumentIdAsync(string documentId, int? excludeId = null) =>
        _context.Users.AnyAsync(u => u.DocumentId == documentId && u.Id != excludeId);

    public Task<int> CountAdminsAsync() =>
        _context.Users.CountAsync(u => u.IsAdmin);

    public async Task AddAsync(User user) =>
        await _context.Users.AddAsync(user);

    public void Remove(User user) =>
        _context.Users.Remove(user);

    public Task SaveChangesAsync() =>
        _context.SaveChangesAsync();
}
