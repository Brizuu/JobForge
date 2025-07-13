namespace JobForge.DbModels;

public class CourseSectionDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Category { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string? VideoUrl { get; set; }
    public decimal? CompletionTime { get; set; }
    public int? CompletionPercentage { get; set; }
    // public bool? IsCompleted { get; set; }
    
}