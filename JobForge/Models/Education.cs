using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobForge.Models;

[Table("UserEducation")]
public class Education
{
    [Key]
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }

    public Guid GeneratedCVId { get; set; }
    [ForeignKey(nameof(GeneratedCVId))]
    public GeneratedCV CV { get; set; }

    public string SchoolName { get; set; } = string.Empty;
    public string? Specialization { get; set; }
    public DateTime EducationDateStart { get; set; }
    public DateTime? EducationDateEnd { get; set; }
}

