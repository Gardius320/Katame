using AutoMapper;
using KatameApi.DTOs.Tasks;
using KatameApi.Middleware;
using KatameApi.Services;

namespace KatameApi.Tests.Services;

public class TaskServiceTests
{
    private static IMapper CreateMapper()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<TaskMappingProfile>(), NullLoggerFactory());
        return config.CreateMapper();
    }

    private static Microsoft.Extensions.Logging.ILoggerFactory NullLoggerFactory() =>
        Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory.Instance;

    private static TaskService CreateService(out FakeTaskRepository repository)
    {
        repository = new FakeTaskRepository();
        return new TaskService(repository, CreateMapper());
    }

    [Fact]
    public async Task CreateAsync_agrega_la_tarea_y_la_devuelve_mapeada()
    {
        var service = CreateService(out _);

        var created = await service.CreateAsync(new CreateTaskItemDto { Title = "Tarea 1", Status = "pending" });

        Assert.Equal("Tarea 1", created.Title);
        Assert.Equal("pending", created.Status);
        Assert.True(created.Id > 0);
    }

    [Fact]
    public async Task DeleteAsync_marca_la_tarea_como_eliminada_y_desaparece_del_listado()
    {
        var service = CreateService(out _);
        var created = await service.CreateAsync(new CreateTaskItemDto { Title = "Tarea 1", Status = "pending" });

        await service.DeleteAsync(created.Id);
        var all = await service.GetAllAsync();

        Assert.Empty(all);
    }

    [Fact]
    public async Task DeleteAsync_lanza_ApiException_404_si_la_tarea_no_existe()
    {
        var service = CreateService(out _);

        var exception = await Assert.ThrowsAsync<ApiException>(() => service.DeleteAsync(999));

        Assert.Equal(System.Net.HttpStatusCode.NotFound, exception.StatusCode);
    }

    [Fact]
    public async Task UpdateAsync_actualiza_los_campos_de_la_tarea()
    {
        var service = CreateService(out _);
        var created = await service.CreateAsync(new CreateTaskItemDto { Title = "Tarea 1", Status = "pending" });

        var updated = await service.UpdateAsync(created.Id, new UpdateTaskItemDto { Title = "Tarea actualizada", Status = "done" });

        Assert.Equal("Tarea actualizada", updated.Title);
        Assert.Equal("done", updated.Status);
    }
}
