using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobForge.Models;

[Table("UserCourses")]
public class UserCourse
{
    [Key]
    public Guid CourseId { get; set; }
    public Guid UserId { get; set; }
    
    public int? LastCompletedSectionId { get; set; }
    
    public Guid GeneratedCVId { get; set; }
    [ForeignKey(nameof(GeneratedCVId))]
    public GeneratedCV CV { get; set; }
    
    public bool isCompleted { get; set; } = false;
    
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public string? Institution { get; set; }
    public decimal? CompletionTime { get; set; }
    public string? Category { get; set; } = string.Empty;
    public bool? Verified { get; set; } = false;
    public double? CompletionPercentage { get; set; }
}

