namespace KatameApi.Models;

public class Subscription : BaseEntity, IUserOwned
{
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime RenewalDate { get; set; }
    public bool ReminderEnabled { get; set; }
}
