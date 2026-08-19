namespace KatameApi.DTOs.Finance;

public class CreateBudgetDto
{
    public string Category { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Period { get; set; } = string.Empty;
    public DateTime AnchorDate { get; set; }
}
