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
    private readonly IMapper _mapper;

    public CreditCardService(ICreditCardRepository creditCardRepository, IMapper mapper)
    {
        _creditCardRepository = creditCardRepository;
        _mapper = mapper;
    }

    public async Task<List<CreditCardDto>> GetAllAsync()
    {
        var cards = await _creditCardRepository.GetAllAsync();
        return _mapper.Map<List<CreditCardDto>>(cards);
    }

    public async Task<CreditCardDto> CreateAsync(CreateCreditCardDto request)
    {
        var card = new CreditCard
        {
            Name = request.Name,
            StatementDay = request.StatementDay,
            PaymentDay = request.PaymentDay,
            CreditLimit = request.CreditLimit,
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
