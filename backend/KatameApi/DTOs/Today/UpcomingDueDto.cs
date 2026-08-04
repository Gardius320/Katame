namespace KatameApi.DTOs.Today;

public class UpcomingDueDto
{
    public string Type { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public decimal? Amount { get; set; }
}

public static class UpcomingDueType
{
    public const string Obligation = "obligation";
    public const string CreditCard = "credit_card";
    public const string Subscription = "subscription";
}
