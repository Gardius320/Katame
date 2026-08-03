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

    public Task<User?> GetByRefreshTokenAsync(string refreshToken) =>
        _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

    public Task<bool> ExistsByUsernameAsync(string username) =>
        _context.Users.AnyAsync(u => u.Username == username);

    public async Task AddAsync(User user) =>
        await _context.Users.AddAsync(user);

    public Task SaveChangesAsync() =>
        _context.SaveChangesAsync();
}
