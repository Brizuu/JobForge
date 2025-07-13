using JobForge.Models;

namespace JobForge.DbModels;

public class GeneratedCVDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    public string? PhoneNumber { get; set; }
    public string? EmailAddress { get; set; }
    public string? LinkedinUrl { get; set; }
    public string? Summary { get; set; }

    public List<EducationDto>? Educations { get; set; }
    public List<WorkExperienceDto>? WorkExperiences { get; set; }
    public List<LanguageDto>? Languages { get; set; }
    public List<SoftSkillsDto>? SoftSkills { get; set; }
    public List<TechnicalSkillsDto>? TechnicalSkills { get; set; }
    public List<InterestsDto>? Interests { get; set; }
    public List<UserCourseVerifiedDto> UserCourse { get; set; }
}