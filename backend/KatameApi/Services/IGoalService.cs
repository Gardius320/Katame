using KatameApi.DTOs.Goals;

namespace KatameApi.Services;

public interface IGoalService
{
    Task<List<GoalDto>> GetAllAsync();
    Task<GoalDto> CreateAsync(CreateGoalDto request);
    Task<GoalDto> UpdateAsync(int id, UpdateGoalDto request);
    Task DeleteAsync(int id);
}
