using Microsoft.EntityFrameworkCore;
using KatameApi.Data;
using KatameApi.Models;

namespace KatameApi.Repositories;

public class CreditCardRepository : ICreditCardRepository
{
    private readonly KatameDbContext _context;

    public CreditCardRepository(KatameDbContext context)
    {
        _context = context;
    }

    public Task<List<CreditCard>> GetAllAsync() =>
        _context.CreditCards.OrderBy(c => c.Name).ToListAsync();

    public Task<CreditCard?> GetByIdAsync(int id) =>
        _context.CreditCards.FirstOrDefaultAsync(c => c.Id == id);

    public async Task AddAsync(CreditCard card) =>
        await _context.CreditCards.AddAsync(card);

    public void Remove(CreditCard card) =>
        _context.CreditCards.Remove(card);

    public Task SaveChangesAsync() =>
        _context.SaveChangesAsync();
}
