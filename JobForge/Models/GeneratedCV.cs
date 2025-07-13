using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobForge.Models;

[Table("UserGeneratedCV")]
public class GeneratedCV
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid UserId { get; set; } // Powiązanie z użytkownikiem z JWT
    
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string EmailAddress { get; set; } = string.Empty;
    public string? LinkedinUrl { get; set; }
    public string? Summary { get; set; }
    
    public List<Education> Educations { get; set; } = new();
    public List<WorkExperience> WorkExperiences { get; set; } = new();
    public List<Language> Languages { get; set; } = new();
    public List<SoftSkills> SoftSkills { get; set; } = new();
    public List<TechnicalSkills> TechnicalSkills { get; set; } = new();
    public List<Interests> Interests { get; set; } = new();
    public List<UserCourse> UserCourse { get; set; } = new();
    
    public DateTime? GenerationDate { get; set; }
}
