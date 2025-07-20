namespace JobForge.DbModels;

public class JobOfferDto
{
    public bool IsArchived { get; set; } = false;

    public string JobTitle { get; set; }
    public string Description { get; set; }
    public List<JobOfferLocationDto> Locations { get; set; } = new();
    public string EmploymentType { get; set; }
    public decimal? SalaryFrom { get; set; }
    public decimal? SalaryTo { get; set; }
    public DateTime PostedDate { get; set; }
    public DateTime ExpirationDate { get; set; }
    public string CompanyName { get; set; }
    public string Category { get; set; }
    public string ExperienceLevel { get; set; }
    public List<JobOfferTechnologyDto> Technologies { get; set; } = new();
    public string ApplyLink { get; set; }
    public int PostViews { get; set; }
    public int Applicants { get; set; }
    public int ActiveWorkers { get; set; }
}