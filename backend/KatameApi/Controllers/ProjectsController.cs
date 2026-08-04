using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using KatameApi.DTOs.Projects;
using KatameApi.Services;

namespace KatameApi.Controllers;

[ApiController]
[Route("api/projects")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;
    private readonly IValidator<CreateProjectDto> _createValidator;
    private readonly IValidator<UpdateProjectDto> _updateValidator;

    public ProjectsController(
        IProjectService projectService,
        IValidator<CreateProjectDto> createValidator,
        IValidator<UpdateProjectDto> updateValidator)
    {
        _projectService = projectService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet]
    public async Task<ActionResult<List<ProjectDto>>> GetAll()
    {
        var projects = await _projectService.GetAllAsync();
        return Ok(projects);
    }

    [HttpPost]
    public async Task<ActionResult<ProjectDto>> Create(CreateProjectDto request)
    {
        await _createValidator.ValidateAndThrowAsync(request);
        var project = await _projectService.CreateAsync(request);
        return CreatedAtAction(nameof(GetAll), new { id = project.Id }, project);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProjectDto>> Update(int id, UpdateProjectDto request)
    {
        await _updateValidator.ValidateAndThrowAsync(request);
        var project = await _projectService.UpdateAsync(id, request);
        return Ok(project);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _projectService.DeleteAsync(id);
        return NoContent();
    }
}
