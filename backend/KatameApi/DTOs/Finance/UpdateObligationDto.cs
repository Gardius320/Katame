namespace KatameApi.DTOs.Finance;

public class UpdateObligationDto
{
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime DueDate { get; set; }
    public bool IsRecurring { get; set; }
    public bool IsPaid { get; set; }
}
