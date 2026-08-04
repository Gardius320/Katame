using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using KatameApi.DTOs.Goals;
using KatameApi.Services;

namespace KatameApi.Controllers;

[ApiController]
[Route("api/goals")]
public class GoalsController : ControllerBase
{
    private readonly IGoalService _goalService;
    private readonly IValidator<CreateGoalDto> _createValidator;
    private readonly IValidator<UpdateGoalDto> _updateValidator;

    public GoalsController(
        IGoalService goalService,
        IValidator<CreateGoalDto> createValidator,
        IValidator<UpdateGoalDto> updateValidator)
    {
        _goalService = goalService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet]
    public async Task<ActionResult<List<GoalDto>>> GetAll()
    {
        var goals = await _goalService.GetAllAsync();
        return Ok(goals);
    }

    [HttpPost]
    public async Task<ActionResult<GoalDto>> Create(CreateGoalDto request)
    {
        await _createValidator.ValidateAndThrowAsync(request);
        var goal = await _goalService.CreateAsync(request);
        return CreatedAtAction(nameof(GetAll), new { id = goal.Id }, goal);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<GoalDto>> Update(int id, UpdateGoalDto request)
    {
        await _updateValidator.ValidateAndThrowAsync(request);
        var goal = await _goalService.UpdateAsync(id, request);
        return Ok(goal);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _goalService.DeleteAsync(id);
        return NoContent();
    }
}
