using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using KatameApi.DTOs.Finance;
using KatameApi.Services;

namespace KatameApi.Controllers;

[ApiController]
[Route("api/finance/credit-cards")]
public class CreditCardsController : ControllerBase
{
    private readonly ICreditCardService _creditCardService;
    private readonly IValidator<CreateCreditCardDto> _createValidator;
    private readonly IValidator<UpdateCreditCardDto> _updateValidator;

    public CreditCardsController(
        ICreditCardService creditCardService,
        IValidator<CreateCreditCardDto> createValidator,
        IValidator<UpdateCreditCardDto> updateValidator)
    {
        _creditCardService = creditCardService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet]
    public async Task<ActionResult<List<CreditCardDto>>> GetAll()
    {
        var cards = await _creditCardService.GetAllAsync();
        return Ok(cards);
    }

    [HttpPost]
    public async Task<ActionResult<CreditCardDto>> Create(CreateCreditCardDto request)
    {
        await _createValidator.ValidateAndThrowAsync(request);
        var card = await _creditCardService.CreateAsync(request);
        return CreatedAtAction(nameof(GetAll), new { id = card.Id }, card);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<CreditCardDto>> Update(int id, UpdateCreditCardDto request)
    {
        await _updateValidator.ValidateAndThrowAsync(request);
        var card = await _creditCardService.UpdateAsync(id, request);
        return Ok(card);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _creditCardService.DeleteAsync(id);
        return NoContent();
    }
}
