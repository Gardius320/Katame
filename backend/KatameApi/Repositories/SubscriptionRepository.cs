using Microsoft.EntityFrameworkCore;
using KatameApi.Data;
using KatameApi.Models;

namespace KatameApi.Repositories;

public class SubscriptionRepository : ISubscriptionRepository
{
    private readonly KatameDbContext _context;

    public SubscriptionRepository(KatameDbContext context)
    {
        _context = context;
    }

    public Task<List<Subscription>> GetAllAsync() =>
        _context.Subscriptions.OrderBy(s => s.RenewalDate).ToListAsync();

    public Task<Subscription?> GetByIdAsync(int id) =>
        _context.Subscriptions.FirstOrDefaultAsync(s => s.Id == id);

    public async Task AddAsync(Subscription subscription) =>
        await _context.Subscriptions.AddAsync(subscription);

    public Task SaveChangesAsync() =>
        _context.SaveChangesAsync();
}
