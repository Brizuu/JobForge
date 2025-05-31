using JobForge.DbModels;

namespace JobForge.Models;

public class Course
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public Guid CreatorId { get; set; }

    public string Category { get; set; } = string.Empty;

    public List<CourseSection> Sections { get; set; } = new();
}