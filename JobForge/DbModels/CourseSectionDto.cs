namespace JobForge.DbModels;

public class CourseSectionDto
{
 
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public int CompletionPercentage { get; set; } // 0–100
}