using AutoMapper;
using KatameApi.Models;

namespace KatameApi.DTOs.Tasks;

public class TaskMappingProfile : Profile
{
    public TaskMappingProfile()
    {
        CreateMap<TaskItem, TaskItemDto>();
    }
}
