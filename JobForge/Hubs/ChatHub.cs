using JobForge.Data;
using JobForge.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace JobForge.Hubs;

public class ChatHub : Hub
{
    private readonly AppDbContext _context;

    public ChatHub(AppDbContext context)
    {
        _context = context;
    }

    public async Task SendMessage(Guid toUserId, string message, string? fileUrl = null)
    {
        var senderId = Guid.Parse(Context.UserIdentifier!);

        if (senderId == toUserId)
            throw new HubException("You cannot send a message to yourself.");

        // Ustal kolejność FirstUser i SecondUser, aby mieć unikalną konwersację
        var orderedUsers = new[] { senderId, toUserId }.OrderBy(x => x).ToArray();
        var firstUser = orderedUsers[0];
        var secondUser = orderedUsers[1];

        // Sprawdź czy istnieje link między tymi użytkownikami
        var link = await _context.ChatUserLinks
            .FirstOrDefaultAsync(l => l.FirstUser == firstUser && l.SecoundUser == secondUser);

        if (link == null)
        {
            link = new ChatUserLink
            {
                FirstUser = firstUser,
                SecoundUser = secondUser
            };
            _context.ChatUserLinks.Add(link);
            await _context.SaveChangesAsync();
        }

        var chatMessage = new ChatMessage
        {
            SenderId = senderId,
            ReceiverId = toUserId,
            Content = message,
            FileUrl = fileUrl,
            SentAt = DateTime.UtcNow
        };

        _context.ChatMessages.Add(chatMessage);
        await _context.SaveChangesAsync();

        await Clients.User(toUserId.ToString()).SendAsync("ReceiveMessage", new
        {
            From = senderId,
            Content = message,
            FileUrl = fileUrl,
            SentAt = chatMessage.SentAt
        });
    }
}