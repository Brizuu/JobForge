namespace JobForge.DbModels;

public class GrantApplicationDto
{
    public Guid GrantId { get; set; }
    public string Requirements { get; set; }
    
    public string Justification { get; set; } = string.Empty; 

    public decimal RequestedAmount { get; set; }
    public string ApplicantName { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
}