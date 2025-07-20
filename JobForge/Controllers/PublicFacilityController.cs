using System.Security.Authentication;
using System.Security.Claims;
using JobForge.DbModels;
using JobForge.Models;
using JobForge.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobForge.Controllers;


[ApiController]
[Authorize(Roles = "PublicFacility, Admin")]
[Route("api/[controller]")]
public class PublicFacilityController : ControllerBase
{
    private readonly IPublicFacility _context;

    public PublicFacilityController(IPublicFacility context)
    {
        _context = context;
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterDto dto)
    {
        var user = new ApplicationUser
        {
            Email = dto.Email,
            UserName = dto.Email, // wymagane przez Identity
            FirstName = dto.FirstName,
            LastName = dto.LastName
        };

        var success = await _context.RegisterUserAsync(user, dto.Password, GetUserId());
        return success ? Ok("User registered") : BadRequest("Registration failed");
    }


    [HttpGet("company/users")]
    public async Task<IActionResult> GetCompanyUsers()
    {
        var users = await _context.GetUsersByCompanyIdAsync(GetUserId());
        return Ok(users);
    }

    [HttpPatch("assign-supervisor")]
    public async Task<IActionResult> AssignSupervisor(Guid userId, Guid supervisorId)
    {
        var success = await _context.AssignSupervisorAsync(userId, supervisorId, GetUserId());
        return success ? Ok("Supervisor assigned") : Forbid();
    }

    [HttpGet("details/{userId}")]
    public async Task<IActionResult> GetUserDetails(Guid userId)
    {
        var result = await _context.GetUserDetailsAsync(userId, GetUserId());
        return result != null ? Ok(result) : Forbid();
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetCompanyStats([FromQuery] Guid? userId)
    {
        var stats = await _context.GetStatisticsAsync(userId, GetUserId());
        return Ok(stats);
    }
}