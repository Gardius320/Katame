using KatameApi.DTOs.Finance;

namespace KatameApi.Services;

public interface ICreditCardService
{
    Task<List<CreditCardDto>> GetAllAsync();
    Task<CreditCardDto> CreateAsync(CreateCreditCardDto request);
    Task<CreditCardDto> UpdateAsync(int id, UpdateCreditCardDto request);
    Task DeleteAsync(int id);
}
