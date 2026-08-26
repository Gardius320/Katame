namespace KatameApi.DTOs.Achievements;

public class AchievementDto
{
    public string Key { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool Unlocked { get; set; }
    public DateTime? UnlockedAt { get; set; }
}
