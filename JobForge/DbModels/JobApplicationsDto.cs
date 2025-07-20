namespace JobForge.DbModels;

public class JobApplicationsDto
{
    public int Id { get; set; }
    public int JobOfferId { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public DateTime AppliedAt { get; set; }
    public string Status { get; set; } = string.Empty;
}