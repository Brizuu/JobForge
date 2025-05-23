namespace JobForge.Models;

public class WorkExperience
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public string CompanyName { get; set; }
    public DateTime EmploymentDateStart { get; set; }
    public DateTime EmploymentDateEnd { get; set; }
    public string Responsibilities { get; set; }

    public int PersonalInformationId { get; set; }
    public PersonalInformation PersonalInformation { get; set; }
}
