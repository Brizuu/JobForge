using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobForge.Models;

[Table("UserPersonalInformation")]
public class PersonalInformation
{
    [Key]
    public int Id { get; set; }
    public Guid UserId { get; set; }
    
    public Guid GeneratedCVId { get; set; }
    [ForeignKey(nameof(GeneratedCVId))]
    public GeneratedCV CV { get; set; }
    
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string EmailAddress { get; set; }
    public string? LinkedinUrl { get; set; }
    public string? Summary { get; set; }
}
