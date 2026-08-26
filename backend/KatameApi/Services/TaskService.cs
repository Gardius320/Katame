using System.Net;
using AutoMapper;
using KatameApi.DTOs.Tasks;
using KatameApi.Extensions;
using KatameApi.Middleware;
using KatameApi.Models;
using KatameApi.Repositories;

namespace KatameApi.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IMapper _mapper;

    public TaskService(ITaskRepository taskRepository, IMapper mapper)
    {
        _taskRepository = taskRepository;
        _mapper = mapper;
    }

    public async Task<List<TaskItemDto>> GetAllAsync()
    {
        var tasks = await _taskRepository.GetAllAsync();
        return _mapper.Map<List<TaskItemDto>>(tasks);
    }

    public async Task<TaskItemDto> CreateAsync(CreateTaskItemDto request)
    {
        var task = new TaskItem
        {
            Title = request.Title,
            Status = request.Status,
            Date = request.Date?.AsUtc(),
            ProjectId = request.ProjectId,
        };

        await _taskRepository.AddAsync(task);
        await _taskRepository.SaveChangesAsync();

        return _mapper.Map<TaskItemDto>(task);
    }

    public async Task<TaskItemDto> UpdateAsync(int id, UpdateTaskItemDto request)
    {
        var task = await GetTaskOrThrowAsync(id);

        task.Title = request.Title;
        task.Status = request.Status;
        task.Date = request.Date?.AsUtc();
        task.ProjectId = request.ProjectId;

        await _taskRepository.SaveChangesAsync();

        return _mapper.Map<TaskItemDto>(task);
    }

    public async Task DeleteAsync(int id)
    {
        var task = await GetTaskOrThrowAsync(id);
        task.IsDeleted = true;
        await _taskRepository.SaveChangesAsync();
    }

    private async Task<TaskItem> GetTaskOrThrowAsync(int id)
    {
        var task = await _taskRepository.GetByIdAsync(id);
        if (task is null)
        {
            throw new ApiException("La tarea no existe.", HttpStatusCode.NotFound);
        }

        return task;
    }
}
