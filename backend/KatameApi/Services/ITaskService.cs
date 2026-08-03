using KatameApi.DTOs.Tasks;

namespace KatameApi.Services;

public interface ITaskService
{
    Task<List<TaskItemDto>> GetAllAsync();
    Task<TaskItemDto> CreateAsync(CreateTaskItemDto request);
    Task<TaskItemDto> UpdateAsync(int id, UpdateTaskItemDto request);
    Task DeleteAsync(int id);
}
