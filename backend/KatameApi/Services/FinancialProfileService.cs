using AutoMapper;
using KatameApi.DTOs.Finance;
using KatameApi.Repositories;

namespace KatameApi.Services;

public class FinancialProfileService : IFinancialProfileService
{
    private readonly IFinancialProfileRepository _financialProfileRepository;
    private readonly IMapper _mapper;

    public FinancialProfileService(IFinancialProfileRepository financialProfileRepository, IMapper mapper)
    {
        _financialProfileRepository = financialProfileRepository;
        _mapper = mapper;
    }

    // Si el usuario todavía no ha configurado su ingreso, se devuelve en 0 en vez
    // de un 404 -- así el frontend no tiene que manejar un caso "no existe todavía".
    public async Task<FinancialProfileDto> GetAsync()
    {
        var profile = await _financialProfileRepository.GetAsync();
        return profile is null
            ? new FinancialProfileDto { MonthlyIncome = 0 }
            : _mapper.Map<FinancialProfileDto>(profile);
    }

    public async Task<FinancialProfileDto> UpdateAsync(UpdateFinancialProfileDto request)
    {
        var profile = await _financialProfileRepository.UpsertAsync(request.MonthlyIncome);
        return _mapper.Map<FinancialProfileDto>(profile);
    }
}
