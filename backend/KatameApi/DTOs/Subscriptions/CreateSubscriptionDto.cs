namespace KatameApi.DTOs.Subscriptions;

public class CreateSubscriptionDto
{
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime RenewalDate { get; set; }
    public bool ReminderEnabled { get; set; }
}
