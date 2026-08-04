using System.Net;
using AutoMapper;
using KatameApi.DTOs.Finance;
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
            DueDate = request.DueDate?.ToUniversalTime(),
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
        goal.DueDate = request.DueDate?.ToUniversalTime();

        await _savingsGoalRepository.SaveChangesAsync();

        return _mapper.Map<SavingsGoalDto>(goal);
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
