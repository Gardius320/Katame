using KatameApi.Models;

namespace KatameApi.Repositories;

public class TransactionFilter
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Category { get; set; }
    public int? CreditCardId { get; set; }
}

public interface ITransactionRepository
{
    Task<(List<Transaction> Items, int TotalCount)> GetPagedAsync(TransactionFilter filter, int page, int pageSize);
    Task<List<Transaction>> GetAllAsync(TransactionFilter filter);
    Task<Transaction?> GetByIdAsync(int id);
    Task AddAsync(Transaction transaction);
    Task SaveChangesAsync();
}
