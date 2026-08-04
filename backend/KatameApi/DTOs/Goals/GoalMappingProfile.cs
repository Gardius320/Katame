using AutoMapper;
using KatameApi.Models;

namespace KatameApi.DTOs.Goals;

public class GoalMappingProfile : Profile
{
    public GoalMappingProfile()
    {
        CreateMap<Goal, GoalDto>();
    }
}
