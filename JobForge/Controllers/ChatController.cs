using System.Security.Claims;
using JobForge.Data;
using JobForge.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobForge.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ChatController : ControllerBase
{
    private readonly AppDbContext _context;

    public ChatController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("chat/available")]
    [Authorize]
    public async Task<IActionResult> GetAvailableChatUsers()
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        // Pobierz aplikacje użytkownika (czyli pracownika)
        var userApplications = await _context.JobApplications
            .Include(a => a.JobOffer)
            .Where(a => a.UserId == userId)
            .ToListAsync();

        // Pobierz aplikacje na oferty stworzone przez użytkownika (czyli pracodawcę)
        var employerApplications = await _context.JobApplications
            .Include(a => a.JobOffer)
            .Where(a => a.JobOffer.UserId == userId) // ← poprawione z CreatorId
            .ToListAsync();

        var conversationUsers = new HashSet<Guid>();

        // pracownik —> pracodawca
        conversationUsers.UnionWith(userApplications.Select(a => a.JobOffer.UserId)); // ← poprawione

        // pracodawca —> pracownicy
        conversationUsers.UnionWith(employerApplications.Select(a => a.UserId));

        return Ok(conversationUsers);
    }
}