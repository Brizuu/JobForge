namespace JobForge.Models;

public class Grant
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal Amount { get; set; }
    public DateTime ApplicationDeadline { get; set; }  
    public DateTime ApplicationStartDate { get; set; }
    public DateTime ApplicationEndDate { get; set; }
    public bool IsArchived { get; set; } = true;
    public string FundingOrganization { get; set; } = string.Empty;
    public string EligibilityCriteria { get; set; } = string.Empty;
    public string ApplicationProcess { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public Guid AuthorId { get; set; }
}