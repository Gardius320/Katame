using AutoMapper;
using KatameApi.Models;

namespace KatameApi.DTOs.Training;

public class TrainingMappingProfile : Profile
{
    public TrainingMappingProfile()
    {
        CreateMap<Exercise, ExerciseDto>();
        CreateMap<TrainingDay, TrainingDayDto>();
    }
}
