namespace JobForge.DbModels;

public class CourseDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? Institution { get; set; }
    public decimal? CompletionTime { get; set; }
    public bool? Published { get; set; } = false;
    
    public List<CourseSectionDto?> Sections { get; set; } = new();
}