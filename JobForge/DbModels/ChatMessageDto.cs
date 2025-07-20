namespace JobForge.DbModels;

public class ChatMessageDto
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SenderId { get; set; }
    public Guid ReceiverId { get; set; }
    public string Text { get; set; }
    public string? FileBase64 { get; set; } // plik jako base64 opcjonalnie
    public string? FileName { get; set; }
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
}
