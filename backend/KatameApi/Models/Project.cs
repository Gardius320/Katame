namespace KatameApi.Models;

public class Project : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = ProjectStatus.Active;
}

public static class ProjectStatus
{
    public const string Active = "active";
    public const string OnHold = "on_hold";
    public const string Completed = "completed";

    public static readonly string[] All = { Active, OnHold, Completed };
}
