using KatameApi.Models;
using KatameApi.Repositories;

namespace KatameApi.Tests.Services;

public class FakeCreditCardRepository : ICreditCardRepository
{
    private readonly List<CreditCard> _cards = new();
    private int _nextId = 1;

    public Task<List<CreditCard>> GetAllAsync() => Task.FromResult(_cards.ToList());

    public Task<CreditCard?> GetByIdAsync(int id) =>
        Task.FromResult(_cards.FirstOrDefault(c => c.Id == id));

    public Task AddAsync(CreditCard card)
    {
        card.Id = _nextId++;
        _cards.Add(card);
        return Task.CompletedTask;
    }

    public void Remove(CreditCard card) => _cards.Remove(card);

    public Task SaveChangesAsync() => Task.CompletedTask;
}
