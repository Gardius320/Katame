namespace KatameApi.DTOs.Finance;

public class CreateObligationDto
{
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime DueDate { get; set; }
    public bool IsRecurring { get; set; }
}
