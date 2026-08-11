using Microsoft.EntityFrameworkCore;
using KatameApi.Data;
using KatameApi.Models;

namespace KatameApi.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly KatameDbContext _context;

    public TransactionRepository(KatameDbContext context)
    {
        _context = context;
    }

    private IQueryable<Transaction> ApplyFilter(TransactionFilter filter)
    {
        var query = _context.Transactions.AsQueryable();

        if (filter.StartDate is not null)
        {
            query = query.Where(t => t.Date >= filter.StartDate.Value);
        }

        if (filter.EndDate is not null)
        {
            query = query.Where(t => t.Date <= filter.EndDate.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.Category))
        {
            query = query.Where(t => t.Category == filter.Category);
        }

        if (filter.CreditCardId is not null)
        {
            query = query.Where(t => t.CreditCardId == filter.CreditCardId.Value);
        }

        return query.OrderByDescending(t => t.Date).ThenByDescending(t => t.Id);
    }

    public async Task<(List<Transaction> Items, int TotalCount)> GetPagedAsync(
        TransactionFilter filter, int page, int pageSize)
    {
        var query = ApplyFilter(filter);
        var totalCount = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return (items, totalCount);
    }

    public Task<List<Transaction>> GetAllAsync(TransactionFilter filter) =>
        ApplyFilter(filter).ToListAsync();

    public Task<Transaction?> GetByIdAsync(int id) =>
        _context.Transactions.FirstOrDefaultAsync(t => t.Id == id);

    public async Task AddAsync(Transaction transaction) =>
        await _context.Transactions.AddAsync(transaction);

    public Task SaveChangesAsync() =>
        _context.SaveChangesAsync();
}
