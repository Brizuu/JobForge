using JobForge.DbModels;

public interface IChatService
{
    Task SaveMessageAsync(ChatMessageDto message);
    Task<List<ChatMessageDto>> GetMessageHistoryAsync(Guid userId1, Guid userId2);
    Task AddContactIfNotExistsAsync(Guid userId, Guid contactId);
    
    Task<List<ContactDto>> GetContactsAsync(Guid userId);
}