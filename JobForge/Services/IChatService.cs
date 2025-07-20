using JobForge.DbModels;

public interface IChatService
{
    Task SaveMessageAsync(ChatMessageDto message);
    Task<List<ChatMessageDto>> GetMessageHistoryAsync(Guid userId1, Guid userId2);
}