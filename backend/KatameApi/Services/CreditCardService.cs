using System.Net;
using AutoMapper;
using KatameApi.DTOs.Finance;
using KatameApi.Middleware;
using KatameApi.Models;
using KatameApi.Repositories;

namespace KatameApi.Services;

public class CreditCardService : ICreditCardService
{
    private readonly ICreditCardRepository _creditCardRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IMapper _mapper;

    public CreditCardService(
        ICreditCardRepository creditCardRepository,
        ITransactionRepository transactionRepository,
        IMapper mapper)
    {
        _creditCardRepository = creditCardRepository;
        _transactionRepository = transactionRepository;
        _mapper = mapper;
    }

    public async Task<List<CreditCardDto>> GetAllAsync()
    {
        var cards = await _creditCardRepository.GetAllAsync();
        var today = DateTime.UtcNow.Date;

        var dtos = new List<CreditCardDto>();
        foreach (var card in cards)
        {
            var dto = _mapper.Map<CreditCardDto>(card);
            dto.CycleUsage = await GetCycleUsageAsync(card, today);
            dtos.Add(dto);
        }

        return dtos;
    }

    /// <summary>
    /// Cuánto se lleva gastado con esta tarjeta desde el último corte hasta
    /// hoy (el ciclo que todavía está abierto).
    /// </summary>
    private async Task<decimal> GetCycleUsageAsync(CreditCard card, DateTime today)
    {
        var lastStatementDate = BillingCycle.GetLastOccurrenceOnOrBefore(today, card.StatementDay);

        var transactions = await _transactionRepository.GetAllAsync(new TransactionFilter
        {
            CreditCardId = card.Id,
            StartDate = lastStatementDate.AddDays(1),
            EndDate = today,
        });

        return transactions.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);
    }

    public async Task<CreditCardDto> CreateAsync(CreateCreditCardDto request)
    {
        var card = new CreditCard
        {
            Name = request.Name,
            StatementDay = request.StatementDay,
            PaymentDay = request.PaymentDay,
            CreditLimit = request.CreditLimit,
            LogoDataUrl = request.LogoDataUrl,
            Bank = request.Bank,
        };

        await _creditCardRepository.AddAsync(card);
        await _creditCardRepository.SaveChangesAsync();

        return _mapper.Map<CreditCardDto>(card);
    }

    public async Task<CreditCardDto> UpdateAsync(int id, UpdateCreditCardDto request)
    {
        var card = await GetCardOrThrowAsync(id);

        card.Name = request.Name;
        card.StatementDay = request.StatementDay;
        card.PaymentDay = request.PaymentDay;
        card.CreditLimit = request.CreditLimit;
        card.LogoDataUrl = request.LogoDataUrl;
        card.Bank = request.Bank;

        await _creditCardRepository.SaveChangesAsync();

        return _mapper.Map<CreditCardDto>(card);
    }

    public async Task DeleteAsync(int id)
    {
        var card = await GetCardOrThrowAsync(id);
        _creditCardRepository.Remove(card);
        await _creditCardRepository.SaveChangesAsync();
    }

    private async Task<CreditCard> GetCardOrThrowAsync(int id)
    {
        var card = await _creditCardRepository.GetByIdAsync(id);
        if (card is null)
        {
            throw new ApiException("La tarjeta no existe.", HttpStatusCode.NotFound);
        }

        return card;
    }
}
