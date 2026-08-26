using KatameApi.Models;
using KatameApi.Repositories;

namespace KatameApi.Tests.Services;

public class FakeFinancialProfileRepository : IFinancialProfileRepository
{
    private FinancialProfile? _profile;
    private int _nextId = 1;

    public Task<FinancialProfile?> GetAsync() => Task.FromResult(_profile);

    public Task<FinancialProfile> UpsertAsync(decimal monthlyIncome)
    {
        if (_profile is not null)
        {
            _profile.MonthlyIncome = monthlyIncome;
            return Task.FromResult(_profile);
        }

        _profile = new FinancialProfile { Id = _nextId++, MonthlyIncome = monthlyIncome };
        return Task.FromResult(_profile);
    }
}
