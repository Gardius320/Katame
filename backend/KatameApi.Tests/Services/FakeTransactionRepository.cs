using KatameApi.Models;
using KatameApi.Repositories;

namespace KatameApi.Tests.Services;

public class FakeTransactionRepository : ITransactionRepository
{
    private readonly List<Transaction> _transactions = new();
    private int _nextId = 1;

    private static IEnumerable<Transaction> ApplyFilter(IEnumerable<Transaction> source, TransactionFilter filter)
    {
        var query = source.Where(t => !t.IsDeleted);

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

        return query.OrderByDescending(t => t.Date).ThenByDescending(t => t.Id);
    }

    public Task<(List<Transaction> Items, int TotalCount)> GetPagedAsync(TransactionFilter filter, int page, int pageSize)
    {
        var filtered = ApplyFilter(_transactions, filter).ToList();
        var items = filtered.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return Task.FromResult((items, filtered.Count));
    }

    public Task<List<Transaction>> GetAllAsync(TransactionFilter filter) =>
        Task.FromResult(ApplyFilter(_transactions, filter).ToList());

    public Task<Transaction?> GetByIdAsync(int id) =>
        Task.FromResult(_transactions.FirstOrDefault(t => t.Id == id && !t.IsDeleted));

    public Task AddAsync(Transaction transaction)
    {
        transaction.Id = _nextId++;
        _transactions.Add(transaction);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync() => Task.CompletedTask;
}
