using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using KatameApi.DTOs.Common;
using KatameApi.DTOs.Finance;
using KatameApi.Repositories;
using KatameApi.Services;

namespace KatameApi.Controllers;

[ApiController]
[Route("api/finance/transactions")]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionService _transactionService;
    private readonly IValidator<CreateTransactionDto> _createValidator;
    private readonly IValidator<UpdateTransactionDto> _updateValidator;

    public TransactionsController(
        ITransactionService transactionService,
        IValidator<CreateTransactionDto> createValidator,
        IValidator<UpdateTransactionDto> updateValidator)
    {
        _transactionService = transactionService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<TransactionDto>>> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] string? category = null,
        [FromQuery] int? creditCardId = null)
    {
        var filter = BuildFilter(startDate, endDate, category, creditCardId);
        var result = await _transactionService.GetPagedAsync(filter, NormalizePage(page), NormalizePageSize(pageSize));
        return Ok(result);
    }

    [HttpGet("export")]
    public async Task<IActionResult> Export(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] string? category = null,
        [FromQuery] int? creditCardId = null)
    {
        var filter = BuildFilter(startDate, endDate, category, creditCardId);
        var csv = await _transactionService.ExportToCsvAsync(filter);
        var bytes = Encoding.UTF8.GetBytes(csv);
        return File(bytes, "text/csv", "transactions.csv");
    }

    [HttpPost]
    public async Task<ActionResult<TransactionDto>> Create(CreateTransactionDto request)
    {
        await _createValidator.ValidateAndThrowAsync(request);
        var transaction = await _transactionService.CreateAsync(request);
        return CreatedAtAction(nameof(GetPaged), new { id = transaction.Id }, transaction);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TransactionDto>> Update(int id, UpdateTransactionDto request)
    {
        await _updateValidator.ValidateAndThrowAsync(request);
        var transaction = await _transactionService.UpdateAsync(id, request);
        return Ok(transaction);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _transactionService.DeleteAsync(id);
        return NoContent();
    }

    private static TransactionFilter BuildFilter(
        DateTime? startDate, DateTime? endDate, string? category, int? creditCardId) => new()
    {
        StartDate = startDate,
        EndDate = endDate,
        Category = category,
        CreditCardId = creditCardId,
    };

    private static int NormalizePage(int page) => page < 1 ? 1 : page;

    private static int NormalizePageSize(int pageSize) => pageSize switch
    {
        < 1 => 20,
        > 100 => 100,
        _ => pageSize,
    };
}
