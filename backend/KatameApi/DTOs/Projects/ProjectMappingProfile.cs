using AutoMapper;
using KatameApi.Models;

namespace KatameApi.DTOs.Projects;

public class ProjectMappingProfile : Profile
{
    public ProjectMappingProfile()
    {
        CreateMap<Project, ProjectDto>();
    }
}
