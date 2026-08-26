using KatameApi.DTOs.Finance;

namespace KatameApi.Services;

public interface IBudgetService
{
    Task<List<BudgetDto>> GetAllAsync();
    Task<BudgetDto> CreateAsync(CreateBudgetDto request);
    Task<BudgetDto> UpdateAsync(int id, UpdateBudgetDto request);
    Task DeleteAsync(int id);

    /// <summary>
    /// Categorías de gasto pequeño y frecuente detectadas en lo que va del
    /// mes actual -- ver AntExpenseAnalyzer.
    /// </summary>
    Task<List<AntExpenseDto>> GetAntExpensesAsync();
}
