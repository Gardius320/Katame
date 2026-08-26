using System.Net;
using AutoMapper;
using KatameApi.DTOs.Finance;
using KatameApi.Extensions;
using KatameApi.Middleware;
using KatameApi.Models;
using KatameApi.Repositories;

namespace KatameApi.Services;

public class ObligationService : IObligationService
{
    private readonly IObligationRepository _obligationRepository;
    private readonly IMapper _mapper;

    public ObligationService(IObligationRepository obligationRepository, IMapper mapper)
    {
        _obligationRepository = obligationRepository;
        _mapper = mapper;
    }

    public async Task<List<ObligationDto>> GetAllAsync()
    {
        var obligations = await _obligationRepository.GetAllAsync();
        return _mapper.Map<List<ObligationDto>>(obligations);
    }

    public async Task<ObligationDto> CreateAsync(CreateObligationDto request)
    {
        var obligation = new Obligation
        {
            Name = request.Name,
            Amount = request.Amount,
            DueDate = request.DueDate.AsUtc(),
            IsRecurring = request.IsRecurring,
            RecurrenceFrequency = request.IsRecurring ? request.RecurrenceFrequency : null,
            IsPaid = false,
        };

        await _obligationRepository.AddAsync(obligation);
        await _obligationRepository.SaveChangesAsync();

        return _mapper.Map<ObligationDto>(obligation);
    }

    public async Task<ObligationDto> UpdateAsync(int id, UpdateObligationDto request)
    {
        var obligation = await GetObligationOrThrowAsync(id);

        obligation.Name = request.Name;
        obligation.Amount = request.Amount;
        obligation.DueDate = request.DueDate.AsUtc();
        obligation.IsRecurring = request.IsRecurring;
        obligation.RecurrenceFrequency = request.IsRecurring ? request.RecurrenceFrequency : null;
        obligation.IsPaid = request.IsPaid;

        await _obligationRepository.SaveChangesAsync();

        return _mapper.Map<ObligationDto>(obligation);
    }

    public async Task DeleteAsync(int id)
    {
        var obligation = await GetObligationOrThrowAsync(id);
        obligation.IsDeleted = true;
        await _obligationRepository.SaveChangesAsync();
    }

    private async Task<Obligation> GetObligationOrThrowAsync(int id)
    {
        var obligation = await _obligationRepository.GetByIdAsync(id);
        if (obligation is null)
        {
            throw new ApiException("La obligación no existe.", HttpStatusCode.NotFound);
        }

        return obligation;
    }
}
