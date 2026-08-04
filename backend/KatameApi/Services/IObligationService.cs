using KatameApi.DTOs.Finance;

namespace KatameApi.Services;

public interface IObligationService
{
    Task<List<ObligationDto>> GetAllAsync();
    Task<ObligationDto> CreateAsync(CreateObligationDto request);
    Task<ObligationDto> UpdateAsync(int id, UpdateObligationDto request);
    Task DeleteAsync(int id);
}
