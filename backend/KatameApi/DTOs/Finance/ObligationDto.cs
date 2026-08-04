namespace KatameApi.DTOs.Finance;

public class ObligationDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime DueDate { get; set; }
    public bool IsRecurring { get; set; }
    public bool IsPaid { get; set; }
}
