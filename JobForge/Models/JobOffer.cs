namespace JobForge.Models;

public class JobOffer
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public bool IsArchived { get; set; } = false;

    public string JobTitle { get; set; }
    public string Description { get; set; }
    public string Location { get; set; }
    public string EmploymentType { get; set; }
    public decimal? SalaryFrom { get; set; }
    public decimal? SalaryTo { get; set; }
    public DateTime PostedDate { get; set; }
    public DateTime ExpirationDate { get; set; }
    public string CompanyName { get; set; }
    public string Category { get; set; }
    public string ExperienceLevel { get; set; }
    public List<JobOfferTechnology> Technologies { get; set; } = new();
    public string ApplyLink { get; set; }
    public int PostViews { get; set; }
    public int Applicants { get; set; }
    public int ActiveWorkers { get; set; }
}