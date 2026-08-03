using System.Net;
using AutoMapper;
using KatameApi.DTOs.Training;
using KatameApi.Middleware;
using KatameApi.Models;
using KatameApi.Repositories;

namespace KatameApi.Services;

public class TrainingService : ITrainingService
{
    private readonly ITrainingDayRepository _trainingDayRepository;
    private readonly IMapper _mapper;

    public TrainingService(ITrainingDayRepository trainingDayRepository, IMapper mapper)
    {
        _trainingDayRepository = trainingDayRepository;
        _mapper = mapper;
    }

    public async Task<List<TrainingDayDto>> GetAllDaysAsync()
    {
        var days = await _trainingDayRepository.GetAllAsync();
        var ordered = days.OrderBy(d => MondayFirstIndex(d.DayOfWeek)).ToList();
        return _mapper.Map<List<TrainingDayDto>>(ordered);
    }

    public async Task<TrainingDayDto> CreateDayAsync(CreateTrainingDayDto request)
    {
        var day = new TrainingDay
        {
            DayOfWeek = request.DayOfWeek,
            Title = request.Title,
        };

        await _trainingDayRepository.AddAsync(day);
        await _trainingDayRepository.SaveChangesAsync();

        return _mapper.Map<TrainingDayDto>(day);
    }

    public async Task<TrainingDayDto> UpdateDayAsync(int id, UpdateTrainingDayDto request)
    {
        var day = await GetDayOrThrowAsync(id);

        day.DayOfWeek = request.DayOfWeek;
        day.Title = request.Title;

        await _trainingDayRepository.SaveChangesAsync();

        return _mapper.Map<TrainingDayDto>(day);
    }

    public async Task DeleteDayAsync(int id)
    {
        var day = await GetDayOrThrowAsync(id);
        _trainingDayRepository.Remove(day);
        await _trainingDayRepository.SaveChangesAsync();
    }

    public async Task<ExerciseDto> AddExerciseAsync(int dayId, CreateExerciseDto request)
    {
        var day = await GetDayOrThrowAsync(dayId);

        var exercise = new Exercise
        {
            TrainingDayId = dayId,
            Name = request.Name,
            SetsReps = request.SetsReps,
        };

        day.Exercises.Add(exercise);
        await _trainingDayRepository.SaveChangesAsync();

        return _mapper.Map<ExerciseDto>(exercise);
    }

    public async Task<ExerciseDto> UpdateExerciseAsync(int dayId, int exerciseId, UpdateExerciseDto request)
    {
        var exercise = await GetExerciseOrThrowAsync(dayId, exerciseId);

        exercise.Name = request.Name;
        exercise.SetsReps = request.SetsReps;

        await _trainingDayRepository.SaveChangesAsync();

        return _mapper.Map<ExerciseDto>(exercise);
    }

    public async Task DeleteExerciseAsync(int dayId, int exerciseId)
    {
        var day = await GetDayOrThrowAsync(dayId);
        var exercise = day.Exercises.FirstOrDefault(e => e.Id == exerciseId);
        if (exercise is null)
        {
            throw new ApiException("El ejercicio no existe.", HttpStatusCode.NotFound);
        }

        day.Exercises.Remove(exercise);
        await _trainingDayRepository.SaveChangesAsync();
    }

    private async Task<TrainingDay> GetDayOrThrowAsync(int id)
    {
        var day = await _trainingDayRepository.GetByIdAsync(id);
        if (day is null)
        {
            throw new ApiException("El día de entrenamiento no existe.", HttpStatusCode.NotFound);
        }

        return day;
    }

    private async Task<Exercise> GetExerciseOrThrowAsync(int dayId, int exerciseId)
    {
        var day = await GetDayOrThrowAsync(dayId);
        var exercise = day.Exercises.FirstOrDefault(e => e.Id == exerciseId);
        if (exercise is null)
        {
            throw new ApiException("El ejercicio no existe.", HttpStatusCode.NotFound);
        }

        return exercise;
    }

    private static int MondayFirstIndex(DayOfWeek dayOfWeek) => ((int)dayOfWeek + 6) % 7;
}
