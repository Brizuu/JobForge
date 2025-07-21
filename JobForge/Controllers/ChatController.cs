using System.Security.Claims;
using JobForge.DbModels;
using JobForge.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ChatController(IChatService chatService, IHttpContextAccessor httpContextAccessor)
    {
        _chatService = chatService;
        _httpContextAccessor = httpContextAccessor;
    }

    private Guid GetUserId()
    {
        var userIdString = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdString!);
    }

    [HttpGet("contacts")]
    public async Task<ActionResult<List<ContactDto>>> GetContacts()
    {
        var currentUserId = GetUserId();
    
        var contacts = await _chatService.GetContactsAsync(currentUserId);
        return Ok(contacts);
    }

    
    
    [HttpGet("history/{otherUserId}")]
    public async Task<IActionResult> GetHistory(Guid otherUserId)
    {
        var currentUserId = GetUserId();

        var messages = await _chatService.GetMessageHistoryAsync(currentUserId, otherUserId);

        return Ok(messages);
    }
}
