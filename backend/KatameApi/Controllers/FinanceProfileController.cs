using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using KatameApi.DTOs.Finance;
using KatameApi.Services;

namespace KatameApi.Controllers;

[ApiController]
[Route("api/finance/profile")]
public class FinanceProfileController : ControllerBase
{
    private readonly IFinancialProfileService _financialProfileService;
    private readonly IValidator<UpdateFinancialProfileDto> _updateValidator;

    public FinanceProfileController(
        IFinancialProfileService financialProfileService,
        IValidator<UpdateFinancialProfileDto> updateValidator)
    {
        _financialProfileService = financialProfileService;
        _updateValidator = updateValidator;
    }

    [HttpGet]
    public async Task<ActionResult<FinancialProfileDto>> Get()
    {
        var profile = await _financialProfileService.GetAsync();
        return Ok(profile);
    }

    [HttpPut]
    public async Task<ActionResult<FinancialProfileDto>> Update(UpdateFinancialProfileDto request)
    {
        await _updateValidator.ValidateAndThrowAsync(request);
        var profile = await _financialProfileService.UpdateAsync(request);
        return Ok(profile);
    }
}
