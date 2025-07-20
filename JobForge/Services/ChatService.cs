using JobForge.Data;
using JobForge.DbModels;
using JobForge.Models;
using Microsoft.EntityFrameworkCore;

namespace JobForge.Services;

public class ChatService : IChatService
{
    private readonly AppDbContext _context;

    public ChatService(AppDbContext context)
    {
        _context = context;
    }

    public async Task SaveMessageAsync(ChatMessageDto message)
    {
        var entity = new ChatMessage
        {
            Id = message.Id,
            SenderId = message.SenderId,
            ReceiverId = message.ReceiverId,
            Text = message.Text,
            FileBase64 = message.FileBase64,
            FileName = message.FileName,
            SentAt = message.SentAt
        };
        _context.ChatMessages.Add(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<List<ChatMessageDto>> GetMessageHistoryAsync(Guid userId1, Guid userId2)
    {
        var messages = await _context.ChatMessages
            .Where(m => (m.SenderId == userId1 && m.ReceiverId == userId2) ||
                        (m.SenderId == userId2 && m.ReceiverId == userId1))
            .OrderBy(m => m.SentAt)
            .ToListAsync();

        return messages.Select(m => new ChatMessageDto
        {
            Id = m.Id,
            SenderId = m.SenderId,
            ReceiverId = m.ReceiverId,
            Text = m.Text,
            FileBase64 = m.FileBase64,
            FileName = m.FileName,
            SentAt = m.SentAt
        }).ToList();
    }
}
