using System.Net;
using AutoMapper;
using KatameApi.DTOs.Finance;
using KatameApi.Extensions;
using KatameApi.Middleware;
using KatameApi.Models;
using KatameApi.Repositories;

namespace KatameApi.Services;

public class SavingsGoalService : ISavingsGoalService
{
    private readonly ISavingsGoalRepository _savingsGoalRepository;
    private readonly IMapper _mapper;

    public SavingsGoalService(ISavingsGoalRepository savingsGoalRepository, IMapper mapper)
    {
        _savingsGoalRepository = savingsGoalRepository;
        _mapper = mapper;
    }

    public async Task<List<SavingsGoalDto>> GetAllAsync()
    {
        var goals = await _savingsGoalRepository.GetAllAsync();
        return _mapper.Map<List<SavingsGoalDto>>(goals);
    }

    public async Task<SavingsGoalDto> CreateAsync(CreateSavingsGoalDto request)
    {
        var goal = new SavingsGoal
        {
            Name = request.Name,
            TargetAmount = request.TargetAmount,
            CurrentAmount = request.CurrentAmount,
            DueDate = request.DueDate?.AsUtc(),
            MonthlyContributionTarget = request.MonthlyContributionTarget,
        };

        await _savingsGoalRepository.AddAsync(goal);
        await _savingsGoalRepository.SaveChangesAsync();

        return _mapper.Map<SavingsGoalDto>(goal);
    }

    public async Task<SavingsGoalDto> UpdateAsync(int id, UpdateSavingsGoalDto request)
    {
        var goal = await GetGoalOrThrowAsync(id);

        goal.Name = request.Name;
        goal.TargetAmount = request.TargetAmount;
        goal.CurrentAmount = request.CurrentAmount;
        goal.DueDate = request.DueDate?.AsUtc();
        goal.MonthlyContributionTarget = request.MonthlyContributionTarget;

        await _savingsGoalRepository.SaveChangesAsync();

        return _mapper.Map<SavingsGoalDto>(goal);
    }

    /// <summary>
    /// Suma un aporte al monto actual de la meta, en vez de tener que reescribir
    /// el total a mano en el formulario de edición cada vez que se ahorra algo.
    /// De paso actualiza la racha de meses seguidos aportando a ESTA meta.
    /// </summary>
    public async Task<SavingsGoalDto> ContributeAsync(int id, ContributeSavingsGoalDto request)
    {
        var goal = await GetGoalOrThrowAsync(id);

        goal.CurrentAmount += request.Amount;
        UpdateContributionStreak(goal);
        await _savingsGoalRepository.SaveChangesAsync();

        return _mapper.Map<SavingsGoalDto>(goal);
    }

    // Compara el mes de este aporte contra el mes del último aporte a la misma
    // meta: si es el mismo mes la racha no cambia (ya contaba), si es el mes
    // siguiente sube, y si hubo un salto se reinicia en 1. El récord
    // (LongestStreakMonths) solo puede subir, nunca bajar.
    private static void UpdateContributionStreak(SavingsGoal goal)
    {
        var currentMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        if (goal.LastContributionMonth is null || goal.LastContributionMonth == currentMonth.AddMonths(-1))
        {
            goal.CurrentStreakMonths += 1;
        }
        else if (goal.LastContributionMonth != currentMonth)
        {
            goal.CurrentStreakMonths = 1;
        }
        // si LastContributionMonth == currentMonth, ya se contó este mes: no cambia.

        goal.LongestStreakMonths = Math.Max(goal.LongestStreakMonths, goal.CurrentStreakMonths);
        goal.LastContributionMonth = currentMonth;
    }

    public async Task DeleteAsync(int id)
    {
        var goal = await GetGoalOrThrowAsync(id);
        _savingsGoalRepository.Remove(goal);
        await _savingsGoalRepository.SaveChangesAsync();
    }

    private async Task<SavingsGoal> GetGoalOrThrowAsync(int id)
    {
        var goal = await _savingsGoalRepository.GetByIdAsync(id);
        if (goal is null)
        {
            throw new ApiException("La meta de ahorro no existe.", HttpStatusCode.NotFound);
        }

        return goal;
    }
}
