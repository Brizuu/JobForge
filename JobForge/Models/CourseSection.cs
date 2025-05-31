using System.Text.Json.Serialization;

namespace JobForge.Models;

public class CourseSection
{
    public int Id { get; set; }
    public int CourseId { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public int CompletionPercentage { get; set; }

    [JsonIgnore]
    public Course Course { get; set; } = null!;
}