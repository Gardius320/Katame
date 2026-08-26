using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using KatameApi.DTOs.Finance;
using KatameApi.Services;

namespace KatameApi.Controllers;

[ApiController]
[Route("api/finance/savings")]
public class SavingsController : ControllerBase
{
    private readonly ISavingsGoalService _savingsGoalService;
    private readonly IValidator<CreateSavingsGoalDto> _createValidator;
    private readonly IValidator<UpdateSavingsGoalDto> _updateValidator;
    private readonly IValidator<ContributeSavingsGoalDto> _contributeValidator;

    public SavingsController(
        ISavingsGoalService savingsGoalService,
        IValidator<CreateSavingsGoalDto> createValidator,
        IValidator<UpdateSavingsGoalDto> updateValidator,
        IValidator<ContributeSavingsGoalDto> contributeValidator)
    {
        _savingsGoalService = savingsGoalService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _contributeValidator = contributeValidator;
    }

    [HttpGet]
    public async Task<ActionResult<List<SavingsGoalDto>>> GetAll()
    {
        var goals = await _savingsGoalService.GetAllAsync();
        return Ok(goals);
    }

    [HttpPost]
    public async Task<ActionResult<SavingsGoalDto>> Create(CreateSavingsGoalDto request)
    {
        await _createValidator.ValidateAndThrowAsync(request);
        var goal = await _savingsGoalService.CreateAsync(request);
        return CreatedAtAction(nameof(GetAll), new { id = goal.Id }, goal);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<SavingsGoalDto>> Update(int id, UpdateSavingsGoalDto request)
    {
        await _updateValidator.ValidateAndThrowAsync(request);
        var goal = await _savingsGoalService.UpdateAsync(id, request);
        return Ok(goal);
    }

    [HttpPost("{id:int}/contribute")]
    public async Task<ActionResult<SavingsGoalDto>> Contribute(int id, ContributeSavingsGoalDto request)
    {
        await _contributeValidator.ValidateAndThrowAsync(request);
        var goal = await _savingsGoalService.ContributeAsync(id, request);
        return Ok(goal);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _savingsGoalService.DeleteAsync(id);
        return NoContent();
    }
}
