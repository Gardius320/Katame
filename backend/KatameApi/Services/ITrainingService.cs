using KatameApi.DTOs.Training;

namespace KatameApi.Services;

public interface ITrainingService
{
    Task<List<TrainingDayDto>> GetAllDaysAsync();
    Task<TrainingDayDto> CreateDayAsync(CreateTrainingDayDto request);
    Task<TrainingDayDto> UpdateDayAsync(int id, UpdateTrainingDayDto request);
    Task DeleteDayAsync(int id);
    Task<ExerciseDto> AddExerciseAsync(int dayId, CreateExerciseDto request);
    Task<ExerciseDto> UpdateExerciseAsync(int dayId, int exerciseId, UpdateExerciseDto request);
    Task DeleteExerciseAsync(int dayId, int exerciseId);
    Task<TrainingStreakDto> GetStreakAsync();
    Task<TrainingStreakDto> MarkTodayCompletedAsync();
}
