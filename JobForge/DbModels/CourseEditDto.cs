namespace JobForge.DbModels;

public class CourseEditDto
{
    // public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? Institution { get; set; }
    public decimal? CompletionTime { get; set; }
    public List<CourseSectionDto?> Sections { get; set; } = new();
}