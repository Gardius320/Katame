using System.Net;
using AutoMapper;
using KatameApi.DTOs.Projects;
using KatameApi.Middleware;
using KatameApi.Models;
using KatameApi.Repositories;

namespace KatameApi.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;
    private readonly IMapper _mapper;

    public ProjectService(IProjectRepository projectRepository, IMapper mapper)
    {
        _projectRepository = projectRepository;
        _mapper = mapper;
    }

    public async Task<List<ProjectDto>> GetAllAsync()
    {
        var projects = await _projectRepository.GetAllAsync();
        return _mapper.Map<List<ProjectDto>>(projects);
    }

    public async Task<ProjectDto> CreateAsync(CreateProjectDto request)
    {
        var project = new Project
        {
            Name = request.Name,
            Description = request.Description,
            Status = request.Status,
        };

        await _projectRepository.AddAsync(project);
        await _projectRepository.SaveChangesAsync();

        return _mapper.Map<ProjectDto>(project);
    }

    public async Task<ProjectDto> UpdateAsync(int id, UpdateProjectDto request)
    {
        var project = await GetProjectOrThrowAsync(id);

        project.Name = request.Name;
        project.Description = request.Description;
        project.Status = request.Status;

        await _projectRepository.SaveChangesAsync();

        return _mapper.Map<ProjectDto>(project);
    }

    public async Task DeleteAsync(int id)
    {
        var project = await GetProjectOrThrowAsync(id);
        project.IsDeleted = true;
        await _projectRepository.SaveChangesAsync();
    }

    private async Task<Project> GetProjectOrThrowAsync(int id)
    {
        var project = await _projectRepository.GetByIdAsync(id);
        if (project is null)
        {
            throw new ApiException("El proyecto no existe.", HttpStatusCode.NotFound);
        }

        return project;
    }
}
