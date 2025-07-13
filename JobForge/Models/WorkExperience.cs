using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobForge.Models;

[Table("UserWorkExperience")]
public class WorkExperience
{
    [Key]
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }

    public Guid GeneratedCVId { get; set; }
    [ForeignKey(nameof(GeneratedCVId))]
    public GeneratedCV CV { get; set; }

    public string CompanyName { get; set; } = string.Empty;
    public string PositionTitle { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string EmploymentType { get; set; } = string.Empty;
    public DateTime EmploymentDateStart { get; set; }
    public DateTime? EmploymentDateEnd { get; set; }
    public string? Responsibilities { get; set; }
    public bool? Verified { get; set; }
    public string? TechnologiesUsed { get; set; }
}

