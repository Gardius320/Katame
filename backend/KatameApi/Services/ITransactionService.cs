using KatameApi.DTOs.Common;
using KatameApi.DTOs.Finance;
using KatameApi.Repositories;

namespace KatameApi.Services;

public interface ITransactionService
{
    Task<PagedResult<TransactionDto>> GetPagedAsync(TransactionFilter filter, int page, int pageSize);
    Task<TransactionDto> CreateAsync(CreateTransactionDto request);
    Task<TransactionDto> UpdateAsync(int id, UpdateTransactionDto request);
    Task DeleteAsync(int id);
    Task<string> ExportToCsvAsync(TransactionFilter filter);
}
