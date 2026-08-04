using KatameApi.DTOs.Projects;

namespace KatameApi.Services;

public interface IProjectService
{
    Task<List<ProjectDto>> GetAllAsync();
    Task<ProjectDto> CreateAsync(CreateProjectDto request);
    Task<ProjectDto> UpdateAsync(int id, UpdateProjectDto request);
    Task DeleteAsync(int id);
}
