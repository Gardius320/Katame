using KatameApi.Models;

namespace KatameApi.Repositories;

public interface IObligationRepository
{
    Task<List<Obligation>> GetAllAsync();
    Task<Obligation?> GetByIdAsync(int id);
    Task AddAsync(Obligation obligation);
    Task SaveChangesAsync();
}
