namespace JobForge.DbModels;

public class CourseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public Guid CreatorId { get; set; }
    
    public string Category { get; set; } = string.Empty;

    public List<CourseSectionDto> Sections { get; set; } = new();
}