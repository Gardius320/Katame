namespace KatameApi.DTOs.Subscriptions;

public class UpdateSubscriptionDto
{
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime RenewalDate { get; set; }
    public bool ReminderEnabled { get; set; }
}
