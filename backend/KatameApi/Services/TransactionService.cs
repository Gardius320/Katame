using System.Globalization;
using System.Net;
using System.Text;
using AutoMapper;
using KatameApi.DTOs.Common;
using KatameApi.DTOs.Finance;
using KatameApi.Extensions;
using KatameApi.Middleware;
using KatameApi.Models;
using KatameApi.Repositories;

namespace KatameApi.Services;

public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly ICreditCardRepository _creditCardRepository;
    private readonly IMapper _mapper;

    public TransactionService(
        ITransactionRepository transactionRepository,
        ICreditCardRepository creditCardRepository,
        IMapper mapper)
    {
        _transactionRepository = transactionRepository;
        _creditCardRepository = creditCardRepository;
        _mapper = mapper;
    }

    public async Task<PagedResult<TransactionDto>> GetPagedAsync(TransactionFilter filter, int page, int pageSize)
    {
        var (items, totalCount) = await _transactionRepository.GetPagedAsync(filter, page, pageSize);

        return new PagedResult<TransactionDto>
        {
            Items = _mapper.Map<List<TransactionDto>>(items),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
        };
    }

    public async Task<TransactionDto> CreateAsync(CreateTransactionDto request)
    {
        await EnsureCreditCardExistsAsync(request.CreditCardId);

        var transaction = new Transaction
        {
            Amount = request.Amount,
            Type = request.Type,
            Category = request.Category,
            Date = request.Date.AsUtc(),
            CreditCardId = request.CreditCardId,
        };

        await _transactionRepository.AddAsync(transaction);
        await _transactionRepository.SaveChangesAsync();

        return _mapper.Map<TransactionDto>(transaction);
    }

    public async Task<TransactionDto> UpdateAsync(int id, UpdateTransactionDto request)
    {
        var transaction = await GetTransactionOrThrowAsync(id);
        await EnsureCreditCardExistsAsync(request.CreditCardId);

        transaction.Amount = request.Amount;
        transaction.Type = request.Type;
        transaction.Category = request.Category;
        transaction.Date = request.Date.AsUtc();
        transaction.CreditCardId = request.CreditCardId;

        await _transactionRepository.SaveChangesAsync();

        return _mapper.Map<TransactionDto>(transaction);
    }

    private async Task EnsureCreditCardExistsAsync(int? creditCardId)
    {
        if (creditCardId is null)
        {
            return;
        }

        var card = await _creditCardRepository.GetByIdAsync(creditCardId.Value);
        if (card is null)
        {
            throw new ApiException("La tarjeta no existe.", HttpStatusCode.NotFound);
        }
    }

    public async Task DeleteAsync(int id)
    {
        var transaction = await GetTransactionOrThrowAsync(id);
        transaction.IsDeleted = true;
        await _transactionRepository.SaveChangesAsync();
    }

    public async Task<string> ExportToCsvAsync(TransactionFilter filter)
    {
        var transactions = await _transactionRepository.GetAllAsync(filter);

        var builder = new StringBuilder();
        builder.AppendLine("Id,Amount,Type,Category,Date");

        foreach (var transaction in transactions)
        {
            builder.AppendLine(string.Join(',',
                transaction.Id,
                transaction.Amount.ToString(CultureInfo.InvariantCulture),
                transaction.Type,
                EscapeCsvField(transaction.Category),
                transaction.Date.ToString("O", CultureInfo.InvariantCulture)));
        }

        return builder.ToString();
    }

    private static string EscapeCsvField(string field)
    {
        if (field.Contains(',') || field.Contains('"') || field.Contains('\n'))
        {
            return $"\"{field.Replace("\"", "\"\"")}\"";
        }

        return field;
    }

    private async Task<Transaction> GetTransactionOrThrowAsync(int id)
    {
        var transaction = await _transactionRepository.GetByIdAsync(id);
        if (transaction is null)
        {
            throw new ApiException("La transacción no existe.", HttpStatusCode.NotFound);
        }

        return transaction;
    }
}
