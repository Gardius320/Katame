namespace KatameApi.Models;

public class TaskItem : BaseEntity, IUserOwned
{
    public int UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Status { get; set; } = TaskItemStatus.Pending;
    public DateTime? Date { get; set; }
    public int? ProjectId { get; set; }
}

public static class TaskItemStatus
{
    public const string Pending = "pending";
    public const string InProgress = "in_progress";
    public const string Done = "done";

    public static readonly string[] All = { Pending, InProgress, Done };
}
