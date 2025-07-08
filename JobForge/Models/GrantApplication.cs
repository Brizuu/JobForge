namespace JobForge.Models;

public class GrantApplication
{
    public Guid Id { get; set; }
    public Guid GrantId { get; set; }
    public Guid UserId { get; set; }
    public DateTime AppliedAt { get; set; }
    public string Requirements { get; set; }
}