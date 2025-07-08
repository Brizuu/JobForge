using JobForge.DbModels;
using JobForge.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GrantController : ControllerBase
{
    private readonly IGrantService _service;

    public GrantController(IGrantService service)
    {
        _service = service;
    }

    private Guid GetUserIdFromToken()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
        if (claim == null || !Guid.TryParse(claim.Value, out var userId))
            throw new UnauthorizedAccessException("Invalid token.");
        return userId;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateGrant([FromBody] GrantDto dto)
    {
        var userId = GetUserIdFromToken();
        var created = await _service.CreateGrantAsync(dto, userId);
        return Created(string.Empty, created);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGrant(Guid id)
    {
        var deleted = await _service.DeleteGrantAsync(id);
        return deleted ? NoContent() : NotFound();
    }

    [HttpGet("{id?}")]
    public async Task<IActionResult> GetGrants([FromQuery] Guid? id = null)
    {
        if (id.HasValue)
        {
            var grant = await _service.GetGrantByIdAsync(id.Value);
            if (grant == null) return NotFound();
            return Ok(grant);
        }

        var all = await _service.GetAllGrantsAsync();
        return Ok(all);
    }

    [HttpPost("applications/create")]
    public async Task<IActionResult> CreateApplication([FromBody] GrantApplicationDto dto)
    {
        var userId = GetUserIdFromToken();
        try
        {
            var created = await _service.CreateApplicationAsync(dto, userId);
            return Created(string.Empty, created);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = "GrantNotFound", message = ex.Message });
        }
    }

    [HttpGet("applications/{grantId?}")]
    public async Task<IActionResult> GetApplications([FromQuery] Guid? grantId = null)
    {
        var apps = await _service.GetApplicationsAsync(grantId);
        return Ok(apps);
    }

    [HttpDelete("applications/{id}")]
    public async Task<IActionResult> DeleteApplication(Guid id)
    {
        var deleted = await _service.DeleteApplicationAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
