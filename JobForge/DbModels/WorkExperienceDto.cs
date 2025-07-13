namespace JobForge.DbModels;

public class WorkExperienceDto
{
    public string CompanyName { get; set; }
    
    public string PositionTitle { get; set; }
    public string? Location { get; set; }
    public string EmploymentType { get; set; }
    public DateTime EmploymentDateStart { get; set; }
    public DateTime? EmploymentDateEnd { get; set; }
    public string? Responsibilities { get; set; }
    public string? TechnologiesUsed { get; set; }
}
