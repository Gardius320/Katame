using KatameApi.DTOs.Finance;

namespace KatameApi.Services;

public interface IFinancialProfileService
{
    Task<FinancialProfileDto> GetAsync();
    Task<FinancialProfileDto> UpdateAsync(UpdateFinancialProfileDto request);
}
