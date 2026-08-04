using AutoMapper;
using KatameApi.DTOs.Projects;
using KatameApi.Middleware;
using KatameApi.Models;
using KatameApi.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace KatameApi.Tests.Services;

public class ProjectServiceTests
{
    private static IMapper CreateMapper()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<ProjectMappingProfile>(), NullLoggerFactory.Instance);
        return config.CreateMapper();
    }

    [Fact]
    public async Task ProjectService_crea_actualiza_y_elimina()
    {
        var service = new ProjectService(new FakeProjectRepository(), CreateMapper());

        var project = await service.CreateAsync(new CreateProjectDto
        {
            Name = "Rediseño de la casa",
            Description = "Renovar la cocina y el patio",
            Status = ProjectStatus.Active,
        });

        var updated = await service.UpdateAsync(project.Id, new UpdateProjectDto
        {
            Name = "Rediseño de la casa",
            Description = "Renovar la cocina y el patio",
            Status = ProjectStatus.Completed,
        });

        Assert.Equal(ProjectStatus.Completed, updated.Status);

        await service.DeleteAsync(project.Id);
        Assert.Empty(await service.GetAllAsync());
    }

    [Fact]
    public async Task ProjectService_DeleteAsync_lanza_ApiException_404_si_no_existe()
    {
        var service = new ProjectService(new FakeProjectRepository(), CreateMapper());

        var exception = await Assert.ThrowsAsync<ApiException>(() => service.DeleteAsync(999));

        Assert.Equal(System.Net.HttpStatusCode.NotFound, exception.StatusCode);
    }
}
