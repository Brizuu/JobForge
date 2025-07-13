namespace JobForge.DbModels;

public class UserCourseEditDto
{
    public Guid Id { get; set; }
    public string? Title { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public string? Institution { get; set; }
    public decimal? CompletionTime { get; set; }
    public string? Category { get; set; } = string.Empty;
    public double? CompletionPercentage { get; set; }
}