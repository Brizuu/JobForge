namespace JobForge.DbModels;

public class WorkExperienceDto
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public string CompanyName { get; set; }
    public DateTime EmploymentDateStart { get; set; }
    public DateTime EmploymentDateEnd { get; set; }
    public string Responsibilities { get; set; }
}
