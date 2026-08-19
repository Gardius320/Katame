using System.Net;
using AutoMapper;
using KatameApi.DTOs.Finance;
using KatameApi.Middleware;
using KatameApi.Models;
using KatameApi.Repositories;

namespace KatameApi.Services;

public class BudgetService : IBudgetService
{
    private readonly IBudgetRepository _budgetRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IMapper _mapper;

    public BudgetService(
        IBudgetRepository budgetRepository,
        ITransactionRepository transactionRepository,
        IMapper mapper)
    {
        _budgetRepository = budgetRepository;
        _transactionRepository = transactionRepository;
        _mapper = mapper;
    }

    public async Task<List<BudgetDto>> GetAllAsync()
    {
        var budgets = await _budgetRepository.GetAllAsync();
        var today = DateTime.UtcNow.Date;

        var dtos = new List<BudgetDto>();
        foreach (var budget in budgets)
        {
            var (cycleStart, cycleEnd) = BudgetCycle.GetCurrentCycle(today, budget.Period, budget.AnchorDate);

            var dto = _mapper.Map<BudgetDto>(budget);
            dto.CycleStart = cycleStart;
            dto.CycleEnd = cycleEnd;
            dto.Spent = await GetSpentAsync(budget.Category, cycleStart, today);
            dtos.Add(dto);
        }

        return dtos;
    }

    /// <summary>
    /// Cuánto se lleva gastado en esta categoría desde que arrancó el ciclo
    /// vigente hasta hoy.
    /// </summary>
    private async Task<decimal> GetSpentAsync(string category, DateTime cycleStart, DateTime today)
    {
        var transactions = await _transactionRepository.GetAllAsync(new TransactionFilter
        {
            Category = category,
            StartDate = cycleStart,
            EndDate = today,
        });

        return transactions.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);
    }

    public async Task<BudgetDto> CreateAsync(CreateBudgetDto request)
    {
        var budget = new Budget
        {
            Category = request.Category,
            Amount = request.Amount,
            Period = request.Period,
            AnchorDate = request.AnchorDate,
        };

        await _budgetRepository.AddAsync(budget);
        await _budgetRepository.SaveChangesAsync();

        return await ToDtoAsync(budget);
    }

    public async Task<BudgetDto> UpdateAsync(int id, UpdateBudgetDto request)
    {
        var budget = await GetBudgetOrThrowAsync(id);

        budget.Category = request.Category;
        budget.Amount = request.Amount;
        budget.Period = request.Period;
        budget.AnchorDate = request.AnchorDate;

        await _budgetRepository.SaveChangesAsync();

        return await ToDtoAsync(budget);
    }

    public async Task DeleteAsync(int id)
    {
        var budget = await GetBudgetOrThrowAsync(id);
        _budgetRepository.Remove(budget);
        await _budgetRepository.SaveChangesAsync();
    }

    private async Task<BudgetDto> ToDtoAsync(Budget budget)
    {
        var today = DateTime.UtcNow.Date;
        var (cycleStart, cycleEnd) = BudgetCycle.GetCurrentCycle(today, budget.Period, budget.AnchorDate);

        var dto = _mapper.Map<BudgetDto>(budget);
        dto.CycleStart = cycleStart;
        dto.CycleEnd = cycleEnd;
        dto.Spent = await GetSpentAsync(budget.Category, cycleStart, today);
        return dto;
    }

    private async Task<Budget> GetBudgetOrThrowAsync(int id)
    {
        var budget = await _budgetRepository.GetByIdAsync(id);
        if (budget is null)
        {
            throw new ApiException("El presupuesto no existe.", HttpStatusCode.NotFound);
        }

        return budget;
    }
}
