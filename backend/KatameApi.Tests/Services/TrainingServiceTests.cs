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

    private static TrainingService CreateService() => new(new FakeTrainingDayRepository(), CreateMapper());

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
}
