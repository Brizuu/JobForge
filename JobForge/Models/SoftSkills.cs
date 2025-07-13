using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobForge.Models;

[Table("UserSoftSkills")]
public class SoftSkills
{
    [Key]
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }
    
    public Guid GeneratedCVId { get; set; }
    [ForeignKey(nameof(GeneratedCVId))]
    public GeneratedCV CV { get; set; }
    
    public string SkillName { get; set; }
    public int? ProficiencyLevel { get; set; }
    public string? AdditionalDescription { get; set; }
}