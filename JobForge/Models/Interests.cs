using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobForge.Models;

[Table("UserInterests")]
public class Interests
{
    [Key]
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }
    
    public Guid GeneratedCVId { get; set; }
    [ForeignKey(nameof(GeneratedCVId))]
    public GeneratedCV CV { get; set; }
    
    public string InterestName { get; set; }
    public int? ProficiencyLevel { get; set; }
    public string? AdditionalDescription { get; set; }
}