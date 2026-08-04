using AutoMapper;
using KatameApi.DTOs.Goals;
using KatameApi.Middleware;
using KatameApi.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace KatameApi.Tests.Services;

public class GoalServiceTests
{
    private static IMapper CreateMapper()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<GoalMappingProfile>(), NullLoggerFactory.Instance);
        return config.CreateMapper();
    }

    [Fact]
    public async Task GoalService_crea_actualiza_y_elimina()
    {
        var service = new GoalService(new FakeGoalRepository(), CreateMapper());

        var goal = await service.CreateAsync(new CreateGoalDto
        {
            Title = "Correr 10K",
            Category = "Salud",
            ProgressPercentage = 20,
        });

        var updated = await service.UpdateAsync(goal.Id, new UpdateGoalDto
        {
            Title = "Correr 10K",
            Category = "Salud",
            ProgressPercentage = 75,
        });

        Assert.Equal(75, updated.ProgressPercentage);

        await service.DeleteAsync(goal.Id);
        Assert.Empty(await service.GetAllAsync());
    }

    [Fact]
    public async Task GoalService_DeleteAsync_lanza_ApiException_404_si_no_existe()
    {
        var service = new GoalService(new FakeGoalRepository(), CreateMapper());

        var exception = await Assert.ThrowsAsync<ApiException>(() => service.DeleteAsync(999));

        Assert.Equal(System.Net.HttpStatusCode.NotFound, exception.StatusCode);
    }
}
