using AutoMapper;
using KatameApi.DTOs.Training;
using KatameApi.Middleware;
using KatameApi.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace KatameApi.Tests.Services;

public class TrainingServiceTests
{
    private static IMapper CreateMapper()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<TrainingMappingProfile>(), NullLoggerFactory.Instance);
        return config.CreateMapper();
    }

    private static TrainingService CreateService(
        FakeTrainingDayRepository? dayRepository = null,
        FakeTrainingCompletionRepository? completionRepository = null,
        FakeTrainingStreakRepository? streakRepository = null) =>
        new(
            dayRepository ?? new FakeTrainingDayRepository(),
            completionRepository ?? new FakeTrainingCompletionRepository(),
            streakRepository ?? new FakeTrainingStreakRepository(),
            CreateMapper());

    // Marca todos los días de la semana como "planeados" salvo uno, para poder
    // probar que un día sin plan no afecta la racha sin depender de qué día de
    // la semana sea "hoy" en el momento en que corra el test.
    private static async Task<FakeTrainingDayRepository> PlanAllDaysExceptAsync(DayOfWeek? excluded = null)
    {
        var repository = new FakeTrainingDayRepository();
        var service = new TrainingService(repository, new FakeTrainingCompletionRepository(), new FakeTrainingStreakRepository(), CreateMapper());

        foreach (DayOfWeek day in Enum.GetValues<DayOfWeek>())
        {
            if (day != excluded)
            {
                await service.CreateDayAsync(new CreateTrainingDayDto { DayOfWeek = day, Title = "Día" });
            }
        }

        return repository;
    }

    [Fact]
    public async Task GetAllDaysAsync_ordena_los_dias_empezando_en_lunes()
    {
        var service = CreateService();
        await service.CreateDayAsync(new CreateTrainingDayDto { DayOfWeek = DayOfWeek.Friday, Title = "Pierna" });
        await service.CreateDayAsync(new CreateTrainingDayDto { DayOfWeek = DayOfWeek.Sunday, Title = "Descanso" });
        await service.CreateDayAsync(new CreateTrainingDayDto { DayOfWeek = DayOfWeek.Monday, Title = "Empuje" });

        var days = await service.GetAllDaysAsync();

        Assert.Equal(new[] { DayOfWeek.Monday, DayOfWeek.Friday, DayOfWeek.Sunday }, days.Select(d => d.DayOfWeek));
    }

    [Fact]
    public async Task AddExerciseAsync_agrega_el_ejercicio_al_dia()
    {
        var service = CreateService();
        var day = await service.CreateDayAsync(new CreateTrainingDayDto { DayOfWeek = DayOfWeek.Monday, Title = "Empuje" });

        var exercise = await service.AddExerciseAsync(day.Id, new CreateExerciseDto { Name = "Press banca", SetsReps = "4x8" });

        Assert.Equal("Press banca", exercise.Name);
        Assert.Equal(day.Id, exercise.TrainingDayId);
    }

    [Fact]
    public async Task AddExerciseAsync_lanza_ApiException_404_si_el_dia_no_existe()
    {
        var service = CreateService();

        var exception = await Assert.ThrowsAsync<ApiException>(
            () => service.AddExerciseAsync(999, new CreateExerciseDto { Name = "X", SetsReps = "1x1" }));

        Assert.Equal(System.Net.HttpStatusCode.NotFound, exception.StatusCode);
    }

    [Fact]
    public async Task DeleteExerciseAsync_quita_el_ejercicio_del_dia()
    {
        var service = CreateService();
        var day = await service.CreateDayAsync(new CreateTrainingDayDto { DayOfWeek = DayOfWeek.Monday, Title = "Empuje" });
        var exercise = await service.AddExerciseAsync(day.Id, new CreateExerciseDto { Name = "Press banca", SetsReps = "4x8" });

        await service.DeleteExerciseAsync(day.Id, exercise.Id);
        var days = await service.GetAllDaysAsync();

        Assert.Empty(days.Single(d => d.Id == day.Id).Exercises);
    }

    [Fact]
    public async Task DeleteDayAsync_lanza_ApiException_404_si_el_dia_no_existe()
    {
        var service = CreateService();

        var exception = await Assert.ThrowsAsync<ApiException>(() => service.DeleteDayAsync(999));

        Assert.Equal(System.Net.HttpStatusCode.NotFound, exception.StatusCode);
    }

    [Fact]
    public async Task GetStreakAsync_es_cero_si_no_hay_dias_planeados()
    {
        var service = CreateService();

        var streak = await service.GetStreakAsync();

        Assert.Equal(0, streak.CurrentStreakDays);
    }

    [Fact]
    public async Task MarkTodayCompletedAsync_el_primer_dia_marcado_da_racha_1()
    {
        var dayRepository = await PlanAllDaysExceptAsync();
        var service = CreateService(dayRepository);

        var streak = await service.MarkTodayCompletedAsync();

        Assert.Equal(1, streak.CurrentStreakDays);
        Assert.True(streak.IsNewCompletion);
    }

    [Fact]
    public async Task MarkTodayCompletedAsync_marcar_dos_veces_el_mismo_dia_no_duplica_la_racha()
    {
        var dayRepository = await PlanAllDaysExceptAsync();
        var service = CreateService(dayRepository);

        await service.MarkTodayCompletedAsync();
        var second = await service.MarkTodayCompletedAsync();

        Assert.Equal(1, second.CurrentStreakDays);
        Assert.False(second.IsNewCompletion);
    }

    [Fact]
    public async Task CalculateCurrentStreakAsync_cuenta_dias_planeados_consecutivos()
    {
        var dayRepository = await PlanAllDaysExceptAsync();
        var completionRepository = new FakeTrainingCompletionRepository();
        var today = DateTime.UtcNow.Date;
        await completionRepository.AddAsync(new KatameApi.Models.TrainingCompletion { Date = today.AddDays(-1) });
        await completionRepository.AddAsync(new KatameApi.Models.TrainingCompletion { Date = today.AddDays(-2) });

        var service = CreateService(dayRepository, completionRepository);
        var streak = await service.MarkTodayCompletedAsync();

        Assert.Equal(3, streak.CurrentStreakDays);
    }

    [Fact]
    public async Task CalculateCurrentStreakAsync_un_dia_pasado_sin_completar_rompe_la_racha()
    {
        var dayRepository = await PlanAllDaysExceptAsync();
        var completionRepository = new FakeTrainingCompletionRepository();
        var today = DateTime.UtcNow.Date;
        // Ayer no se completó -- solo hay un registro de hace 3 días.
        await completionRepository.AddAsync(new KatameApi.Models.TrainingCompletion { Date = today.AddDays(-3) });

        var service = CreateService(dayRepository, completionRepository);
        var streak = await service.MarkTodayCompletedAsync();

        Assert.Equal(1, streak.CurrentStreakDays);
    }

    [Fact]
    public async Task CalculateCurrentStreakAsync_un_dia_sin_plan_no_rompe_la_racha()
    {
        var today = DateTime.UtcNow.Date;
        var excluded = today.AddDays(-2).DayOfWeek; // ese día no está planeado
        var dayRepository = await PlanAllDaysExceptAsync(excluded);

        var completionRepository = new FakeTrainingCompletionRepository();
        await completionRepository.AddAsync(new KatameApi.Models.TrainingCompletion { Date = today.AddDays(-1) });
        await completionRepository.AddAsync(new KatameApi.Models.TrainingCompletion { Date = today.AddDays(-3) });
        // today - 2 no tiene registro, pero tampoco está planeado: no debería romper la racha.

        var service = CreateService(dayRepository, completionRepository);
        var streak = await service.MarkTodayCompletedAsync();

        Assert.Equal(3, streak.CurrentStreakDays);
    }

    [Fact]
    public async Task GetStreakAsync_hoy_sin_marcar_todavia_no_rompe_la_racha_de_ayer()
    {
        var dayRepository = await PlanAllDaysExceptAsync();
        var completionRepository = new FakeTrainingCompletionRepository();
        var today = DateTime.UtcNow.Date;
        await completionRepository.AddAsync(new KatameApi.Models.TrainingCompletion { Date = today.AddDays(-1) });

        var service = CreateService(dayRepository, completionRepository);
        var streak = await service.GetStreakAsync();

        Assert.Equal(1, streak.CurrentStreakDays);
    }

    [Fact]
    public async Task UpdateLongestIfHigherAsync_el_record_no_baja_aunque_la_racha_actual_se_reinicie()
    {
        var dayRepository = await PlanAllDaysExceptAsync();
        var completionRepository = new FakeTrainingCompletionRepository();
        var streakRepository = new FakeTrainingStreakRepository();
        var today = DateTime.UtcNow.Date;
        await completionRepository.AddAsync(new KatameApi.Models.TrainingCompletion { Date = today.AddDays(-1) });
        await completionRepository.AddAsync(new KatameApi.Models.TrainingCompletion { Date = today.AddDays(-2) });

        var service = CreateService(dayRepository, completionRepository, streakRepository);
        var firstStreak = await service.MarkTodayCompletedAsync();
        Assert.Equal(3, firstStreak.CurrentStreakDays);
        Assert.Equal(3, firstStreak.LongestStreakDays);

        // Un nuevo servicio "al día siguiente" sin haber entrenado ayer -- la
        // racha actual se rompería, pero el récord debe seguir en 3.
        var completionRepository2 = new FakeTrainingCompletionRepository();
        await completionRepository2.AddAsync(new KatameApi.Models.TrainingCompletion { Date = today });
        var service2 = CreateService(dayRepository, completionRepository2, streakRepository);
        var laterStreak = await service2.GetStreakAsync();

        Assert.Equal(3, laterStreak.LongestStreakDays);
    }
}
