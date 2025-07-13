using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JobForge.DbModels;

namespace JobForge.Models;

public class Course
{
    [Key]
    public Guid Id { get; set; }
    public Guid CreatorId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Institution { get; set; }
    public decimal? CompletionTime { get; set; }
    
    public bool Published { get; set; } = false;

    public string Category { get; set; } = string.Empty;

    public List<CourseSection> Sections { get; set; } = new();
}

