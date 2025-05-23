namespace JobForge.Models;

public class GeneratedCV
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public string ContentJson { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
