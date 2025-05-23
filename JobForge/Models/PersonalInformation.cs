namespace JobForge.Models;

public class PersonalInformation
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public string EmailAddress { get; set; }
    public string LinkedinUrl { get; set; }
    public string Summary { get; set; }

    public List<WorkExperience> WorkExperiences { get; set; }
    public List<Education> Educations { get; set; }
    public List<Language> Languages { get; set; }

    public List<string> TechnicalSkills { get; set; }
    public List<string> SoftSkills { get; set; }
    public List<string> Interests { get; set; }
    public List<string> Certificates { get; set; }
    public List<string> Courses { get; set; }
}
