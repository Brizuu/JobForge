namespace JobForge.Models;

public class GrantApplication
{
    public Guid Id { get; set; }
    public Guid GrantId { get; set; }
    public Guid UserId { get; set; }
    public DateTime AppliedAt { get; set; }
    public string Requirements { get; set; }
    
    public string Justification { get; set; } = string.Empty; 

    public decimal RequestedAmount { get; set; }
    public string ApplicantName { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
}