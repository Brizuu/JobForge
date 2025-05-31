namespace JobForge.DbModels;

public class CourseDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    public string Category { get; set; } = string.Empty;

    public List<CourseSectionDto?> Sections { get; set; } = new();
}