namespace JobForge.Models;

public class ChatMessage
{
    public int Id { get; set; }
    public Guid SenderId { get; set; }
    public Guid ReceiverId { get; set; }
    public string Content { get; set; }
    public string? FileUrl { get; set; }
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
}