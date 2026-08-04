using KatameApi.Models;
using KatameApi.Repositories;

namespace KatameApi.Tests.Services;

public class FakeObligationRepository : IObligationRepository
{
    private readonly List<Obligation> _obligations = new();
    private int _nextId = 1;

    public Task<List<Obligation>> GetAllAsync() =>
        Task.FromResult(_obligations.Where(o => !o.IsDeleted).ToList());

    public Task<Obligation?> GetByIdAsync(int id) =>
        Task.FromResult(_obligations.FirstOrDefault(o => o.Id == id && !o.IsDeleted));

    public Task AddAsync(Obligation obligation)
    {
        obligation.Id = _nextId++;
        _obligations.Add(obligation);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync() => Task.CompletedTask;
}
