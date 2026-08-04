using KatameApi.Models;

namespace KatameApi.Repositories;

public interface ICreditCardRepository
{
    Task<List<CreditCard>> GetAllAsync();
    Task<CreditCard?> GetByIdAsync(int id);
    Task AddAsync(CreditCard card);
    void Remove(CreditCard card);
    Task SaveChangesAsync();
}
