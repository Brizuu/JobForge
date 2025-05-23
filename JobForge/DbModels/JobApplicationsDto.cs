namespace JobForge.DbModels;

public class JobApplicationsDto
{
    public int Id { get; set; }
    public int JobOfferId { get; set; }
    public int UserId { get; set; } // zamienimy na Guid w implementacji
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public string EmailAddress { get; set; }
    public string LinkedinUrl { get; set; }
    public string Summary { get; set; }
    public List<WorkExperienceDto> WorkExperiences { get; set; }
    public List<EducationDto> Educations { get; set; }
    public List<string> TechnicalSkills { get; set; }
    public List<string> SoftSkills { get; set; }
    public List<LanguageDto> Languages { get; set; }
    public List<string> Interests { get; set; }
    public List<string> Certificates { get; set; }
    public List<string> Courses { get; set; }
    public string Status { get; set; }
    public DateTime ApplicationDate { get; set; }
}