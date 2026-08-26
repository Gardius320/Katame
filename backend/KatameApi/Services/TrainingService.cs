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
    private readonly ITrainingCompletionRepository _trainingCompletionRepository;
    private readonly ITrainingStreakRepository _trainingStreakRepository;
    private readonly IMapper _mapper;

    public TrainingService(
        ITrainingDayRepository trainingDayRepository,
        ITrainingCompletionRepository trainingCompletionRepository,
        ITrainingStreakRepository trainingStreakRepository,
        IMapper mapper)
    {
        _trainingDayRepository = trainingDayRepository;
        _trainingCompletionRepository = trainingCompletionRepository;
        _trainingStreakRepository = trainingStreakRepository;
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

    // Solo consulta la racha vigente, sin marcar nada como completado -- se usa
    // para mostrar la insignia "🔥 N días" al abrir la pantalla de Entrenamiento.
    public async Task<TrainingStreakDto> GetStreakAsync()
    {
        var current = await CalculateCurrentStreakAsync();
        var longest = Math.Max(current, await _trainingStreakRepository.GetLongestAsync());

        return new TrainingStreakDto { CurrentStreakDays = current, LongestStreakDays = longest, IsNewCompletion = false };
    }

    // Marca hoy como entrenado (si no se había marcado ya) y devuelve la racha
    // actualizada, para que el frontend pueda mostrar el aviso animado al
    // instante sin pedir el dato dos veces.
    public async Task<TrainingStreakDto> MarkTodayCompletedAsync()
    {
        var today = DateTime.UtcNow.Date;
        var isNewCompletion = !await _trainingCompletionRepository.ExistsForDateAsync(today);

        if (isNewCompletion)
        {
            await _trainingCompletionRepository.AddAsync(new TrainingCompletion { Date = today });
            await _trainingCompletionRepository.SaveChangesAsync();
        }

        var current = await CalculateCurrentStreakAsync();
        var longest = await _trainingStreakRepository.UpdateLongestIfHigherAsync(current);

        return new TrainingStreakDto { CurrentStreakDays = current, LongestStreakDays = longest, IsNewCompletion = isNewCompletion };
    }

    // Cuenta hacia atrás desde hoy cuántos días PLANEADOS seguidos tienen un
    // registro de "completado". Un día sin nada planeado (ej. un descanso) no
    // rompe la racha ni la extiende -- simplemente no cuenta. Si hoy es un día
    // planeado pero todavía no se marcó, tampoco rompe la racha (el día no ha
    // terminado): solo un día PASADO planeado y sin marcar la corta.
    private async Task<int> CalculateCurrentStreakAsync()
    {
        var plannedWeekdays = (await _trainingDayRepository.GetAllAsync())
            .Select(d => d.DayOfWeek)
            .ToHashSet();

        if (plannedWeekdays.Count == 0)
        {
            return 0;
        }

        var completedDates = (await _trainingCompletionRepository.GetAllDatesAsync())
            .Select(d => d.Date)
            .ToHashSet();

        var today = DateTime.UtcNow.Date;
        var cursor = today;
        var streak = 0;

        while ((today - cursor).TotalDays <= 3650)
        {
            if (plannedWeekdays.Contains(cursor.DayOfWeek))
            {
                if (completedDates.Contains(cursor))
                {
                    streak++;
                }
                else if (cursor != today)
                {
                    break;
                }
                // cursor == today y todavía no se completó: el día no ha
                // terminado, no cuenta a favor ni en contra.
            }

            cursor = cursor.AddDays(-1);
        }

        return streak;
    }
}
