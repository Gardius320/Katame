using Microsoft.EntityFrameworkCore;
using KatameApi.Data;
using KatameApi.Models;

namespace KatameApi.Repositories;

public class ObligationRepository : IObligationRepository
{
    private readonly KatameDbContext _context;

    public ObligationRepository(KatameDbContext context)
    {
        _context = context;
    }

    public Task<List<Obligation>> GetAllAsync() =>
        _context.Obligations.OrderBy(o => o.IsPaid).ThenBy(o => o.DueDate).ToListAsync();

    public Task<Obligation?> GetByIdAsync(int id) =>
        _context.Obligations.FirstOrDefaultAsync(o => o.Id == id);

    public async Task AddAsync(Obligation obligation) =>
        await _context.Obligations.AddAsync(obligation);

    public Task SaveChangesAsync() =>
        _context.SaveChangesAsync();
}
