using KatameApi.DTOs.Finance;

namespace KatameApi.Services;

public interface ISavingsGoalService
{
    Task<List<SavingsGoalDto>> GetAllAsync();
    Task<SavingsGoalDto> CreateAsync(CreateSavingsGoalDto request);
    Task<SavingsGoalDto> UpdateAsync(int id, UpdateSavingsGoalDto request);
    Task<SavingsGoalDto> ContributeAsync(int id, ContributeSavingsGoalDto request);
    Task DeleteAsync(int id);
}
