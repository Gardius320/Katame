namespace KatameApi.DTOs.Tasks;

public class UpdateTaskItemDto
{
    public string Title { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime? Date { get; set; }
    public int? ProjectId { get; set; }
}
