using JobForge.DbModels;
using Microsoft.AspNetCore.SignalR;

public class ChatHub : Hub
{
    private readonly IChatService _chatService;

    public ChatHub(IChatService chatService)
    {
        _chatService = chatService;
    }

    // Metoda wywoływana przez klienta do wysłania wiadomości
    public async Task SendMessage(ChatMessageDto message)
    {
        // Zapisz wiadomość w bazie
        await _chatService.SaveMessageAsync(message);

        // Wyślij wiadomość do odbiorcy (jeśli jest online)
        await Clients.User(message.ReceiverId.ToString())
            .SendAsync("ReceiveMessage", message);

        // Możesz też wysłać do nadawcy potwierdzenie
        await Clients.Caller.SendAsync("MessageSent", message);
    }

    // Można override metody OnConnectedAsync, by np. logować połączenia, 
    // lub mapować użytkownika do connectionId (jeśli chcesz targetować po connection)
}