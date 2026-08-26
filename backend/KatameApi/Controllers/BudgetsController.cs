using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using KatameApi.DTOs.Finance;
using KatameApi.Services;

namespace KatameApi.Controllers;

[ApiController]
[Route("api/finance/budgets")]
public class BudgetsController : ControllerBase
{
    private readonly IBudgetService _budgetService;
    private readonly IValidator<CreateBudgetDto> _createValidator;
    private readonly IValidator<UpdateBudgetDto> _updateValidator;

    public BudgetsController(
        IBudgetService budgetService,
        IValidator<CreateBudgetDto> createValidator,
        IValidator<UpdateBudgetDto> updateValidator)
    {
        _budgetService = budgetService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet]
    public async Task<ActionResult<List<BudgetDto>>> GetAll()
    {
        var budgets = await _budgetService.GetAllAsync();
        return Ok(budgets);
    }

    [HttpGet("ant-expenses")]
    public async Task<ActionResult<List<AntExpenseDto>>> GetAntExpenses()
    {
        var antExpenses = await _budgetService.GetAntExpensesAsync();
        return Ok(antExpenses);
    }

    [HttpPost]
    public async Task<ActionResult<BudgetDto>> Create(CreateBudgetDto request)
    {
        await _createValidator.ValidateAndThrowAsync(request);
        var budget = await _budgetService.CreateAsync(request);
        return CreatedAtAction(nameof(GetAll), new { id = budget.Id }, budget);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<BudgetDto>> Update(int id, UpdateBudgetDto request)
    {
        await _updateValidator.ValidateAndThrowAsync(request);
        var budget = await _budgetService.UpdateAsync(id, request);
        return Ok(budget);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _budgetService.DeleteAsync(id);
        return NoContent();
    }
}
