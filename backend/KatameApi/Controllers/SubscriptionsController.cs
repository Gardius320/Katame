using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using KatameApi.DTOs.Subscriptions;
using KatameApi.Services;

namespace KatameApi.Controllers;

[ApiController]
[Route("api/subscriptions")]
public class SubscriptionsController : ControllerBase
{
    private readonly ISubscriptionService _subscriptionService;
    private readonly IValidator<CreateSubscriptionDto> _createValidator;
    private readonly IValidator<UpdateSubscriptionDto> _updateValidator;

    public SubscriptionsController(
        ISubscriptionService subscriptionService,
        IValidator<CreateSubscriptionDto> createValidator,
        IValidator<UpdateSubscriptionDto> updateValidator)
    {
        _subscriptionService = subscriptionService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet]
    public async Task<ActionResult<List<SubscriptionDto>>> GetAll()
    {
        var subscriptions = await _subscriptionService.GetAllAsync();
        return Ok(subscriptions);
    }

    [HttpPost]
    public async Task<ActionResult<SubscriptionDto>> Create(CreateSubscriptionDto request)
    {
        await _createValidator.ValidateAndThrowAsync(request);
        var subscription = await _subscriptionService.CreateAsync(request);
        return CreatedAtAction(nameof(GetAll), new { id = subscription.Id }, subscription);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<SubscriptionDto>> Update(int id, UpdateSubscriptionDto request)
    {
        await _updateValidator.ValidateAndThrowAsync(request);
        var subscription = await _subscriptionService.UpdateAsync(id, request);
        return Ok(subscription);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _subscriptionService.DeleteAsync(id);
        return NoContent();
    }
}
