using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using KatameApi.DTOs.Finance;
using KatameApi.Services;

namespace KatameApi.Controllers;

[ApiController]
[Route("api/finance/obligations")]
public class ObligationsController : ControllerBase
{
    private readonly IObligationService _obligationService;
    private readonly IValidator<CreateObligationDto> _createValidator;
    private readonly IValidator<UpdateObligationDto> _updateValidator;

    public ObligationsController(
        IObligationService obligationService,
        IValidator<CreateObligationDto> createValidator,
        IValidator<UpdateObligationDto> updateValidator)
    {
        _obligationService = obligationService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet]
    public async Task<ActionResult<List<ObligationDto>>> GetAll()
    {
        var obligations = await _obligationService.GetAllAsync();
        return Ok(obligations);
    }

    [HttpPost]
    public async Task<ActionResult<ObligationDto>> Create(CreateObligationDto request)
    {
        await _createValidator.ValidateAndThrowAsync(request);
        var obligation = await _obligationService.CreateAsync(request);
        return CreatedAtAction(nameof(GetAll), new { id = obligation.Id }, obligation);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ObligationDto>> Update(int id, UpdateObligationDto request)
    {
        await _updateValidator.ValidateAndThrowAsync(request);
        var obligation = await _obligationService.UpdateAsync(id, request);
        return Ok(obligation);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _obligationService.DeleteAsync(id);
        return NoContent();
    }
}
