using System.Net;
using AutoMapper;
using KatameApi.DTOs.Goals;
using KatameApi.Middleware;
using KatameApi.Models;
using KatameApi.Repositories;

namespace KatameApi.Services;

public class GoalService : IGoalService
{
    private readonly IGoalRepository _goalRepository;
    private readonly IMapper _mapper;

    public GoalService(IGoalRepository goalRepository, IMapper mapper)
    {
        _goalRepository = goalRepository;
        _mapper = mapper;
    }

    public async Task<List<GoalDto>> GetAllAsync()
    {
        var goals = await _goalRepository.GetAllAsync();
        return _mapper.Map<List<GoalDto>>(goals);
    }

    public async Task<GoalDto> CreateAsync(CreateGoalDto request)
    {
        var goal = new Goal
        {
            Title = request.Title,
            Category = request.Category,
            ProgressPercentage = request.ProgressPercentage,
            DueDate = request.DueDate?.ToUniversalTime(),
        };

        await _goalRepository.AddAsync(goal);
        await _goalRepository.SaveChangesAsync();

        return _mapper.Map<GoalDto>(goal);
    }

    public async Task<GoalDto> UpdateAsync(int id, UpdateGoalDto request)
    {
        var goal = await GetGoalOrThrowAsync(id);

        goal.Title = request.Title;
        goal.Category = request.Category;
        goal.ProgressPercentage = request.ProgressPercentage;
        goal.DueDate = request.DueDate?.ToUniversalTime();

        await _goalRepository.SaveChangesAsync();

        return _mapper.Map<GoalDto>(goal);
    }

    public async Task DeleteAsync(int id)
    {
        var goal = await GetGoalOrThrowAsync(id);
        _goalRepository.Remove(goal);
        await _goalRepository.SaveChangesAsync();
    }

    private async Task<Goal> GetGoalOrThrowAsync(int id)
    {
        var goal = await _goalRepository.GetByIdAsync(id);
        if (goal is null)
        {
            throw new ApiException("La meta no existe.", HttpStatusCode.NotFound);
        }

        return goal;
    }
}
