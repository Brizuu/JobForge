using System.Security.Authentication;
using System.Security.Claims;
using JobForge.DbModels;
using JobForge.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobForge.Controllers;


[ApiController]
[Route("api/[controller]")]
public class PublicFacilityController : ControllerBase
{
    private readonly IPublicFacility _context;

    public PublicFacilityController(IPublicFacility context)
    {
        _context = context;
    }

    [HttpPost("register-user")]
    [Authorize(Roles = "PublicFacility")]
    public async Task<IActionResult> RegisterBySupervisor([FromBody] RegisterDto dto)
    {
        var (success, errors) = await _context.RegisterWithSupervisorAsync(dto, User);

        if (!success)
            return BadRequest(new { errors });

        return Ok(new { message = "User registered with supervisor and assigned to 'free' role" });
    }
    
    [HttpGet("cv/{userId}")]
    [Authorize(Roles = "PublicFacility")]
    public async Task<IActionResult> GetUserCv(Guid userId)
    {
        var supervisorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        try
        {
            var cvData = await _context.GetUserCvForSupervisorAsync(userId, supervisorId);
            return Ok(cvData);
        }
        catch (UnauthorizedAccessException e)
        {
            return Forbid(e.Message);
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(new { error = e.Message });
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(new { error = e.Message });
        }
    }
    
    [HttpGet("applications/{userId:guid}")]
    [Authorize(Roles = "PublicFacility")]
    public async Task<IActionResult> GetUserApplications(Guid userId)
    {
        var supervisorIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(supervisorIdClaim) || !Guid.TryParse(supervisorIdClaim, out var supervisorId))
            return Unauthorized("Nieprawidłowy supervisor ID.");

        try
        {
            var apps = await _context.GetUserApplicationsForSupervisorAsync(userId, supervisorId);
            return Ok(apps);
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (AuthenticationException e)
        {
            return Forbid(e.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "Wystąpił błąd serwera.");
        }
    }
    
    [HttpGet("courses/{userId:guid}")]
    [Authorize(Roles = "PublicFacility")]
    public async Task<IActionResult> GetUserCourses(Guid userId)
    {
        var supervisorIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(supervisorIdClaim) || !Guid.TryParse(supervisorIdClaim, out var supervisorId))
            return Unauthorized("Nieprawidłowy supervisor ID.");

        try
        {
            var courses = await _context.GetUserCoursesForSupervisorAsync(userId, supervisorId);
            return Ok(courses);
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (AuthenticationException e)
        {
            return Forbid(e.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "Wystąpił błąd serwera.");
        }
    }
    
    [HttpGet("employment/{userId:guid}")]
    [Authorize(Roles = "PublicFacility")]
    public async Task<IActionResult> GetUserWorkExperiences(Guid userId)
    {
        var supervisorIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(supervisorIdClaim) || !Guid.TryParse(supervisorIdClaim, out var supervisorId))
            return Unauthorized("Nieprawidłowy supervisor ID.");

        try
        {
            var workHistory = await _context.GetUserWorkExperiencesForSupervisorAsync(userId, supervisorId);
            return Ok(workHistory);
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (AuthenticationException e)
        {
            return Forbid(e.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "Wystąpił błąd serwera.");
        }
    }
}