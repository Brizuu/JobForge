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

    // private Guid GetUserIdFromToken()
    // {
    //     var claim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
    //     if (claim == null || !Guid.TryParse(claim.Value, out var userId))
    //         throw new UnauthorizedAccessException("Invalid token.");
    //     return userId;
    // }
    
    private Guid GetUserId() => Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    [HttpPost("company/grant/create")]
    [Authorize(Roles = "Company, PublicFacility, Admin")]
    public async Task<IActionResult> CreateGrant([FromBody] GrantDto dto)
    {
        var id = await _service.CreateGrantAsync(dto, GetUserId());
        return Ok(new { GrantId = id });
    }

    [HttpPut("company/grant/{id}")]
    [Authorize(Roles = "Company, PublicFacility, Admin")]
    public async Task<IActionResult> UpdateGrant(Guid id, [FromBody] GrantDto dto)
    {
        var result = await _service.UpdateGrantAsync(id, dto, GetUserId());
        return result ? Ok() : NotFound();
    }

    [HttpPatch("company/grant/archive-toggle/{id}")]
    [Authorize(Roles = "Company, PublicFacility, Admin")]
    public async Task<IActionResult> ToggleArchive(Guid id)
    {
        var result = await _service.ToggleArchiveStatusAsync(id, GetUserId());
        return result ? Ok() : NotFound();
    }

    [HttpGet("company/grant/applications/{id}")]
    [Authorize(Roles = "Company, PublicFacility, Admin")]
    public async Task<IActionResult> GetGrantApplications(Guid id)
    {
        var apps = await _service.GetGrantApplicationsForGrantAsync(id, GetUserId());
        return Ok(apps);
    }

    [HttpPatch("company/grant/applications/review/{applicationId}")]
    [Authorize(Roles = "Company, PublicFacility, Admin")]
    public async Task<IActionResult> ReviewGrantApplication(Guid applicationId, [FromQuery] string status)
    {
        var reviewerId = GetUserId(); // lub User.GetUserId() jeśli masz rozszerzenie

        var success = await _service.ReviewGrantApplicationAsync(applicationId, status, reviewerId);
        if (!success)
            return Forbid(); // 403 jeśli grant nie należy do zalogowanego usera

        return Ok(new { Message = "Application status updated successfully." });
    }
    
    [HttpGet("company/my-grants")]
    [Authorize(Roles = "Company, PublicFacility, Admin")]
    public async Task<IActionResult> GetMyGrants()
    {
        var userId = GetUserId();
        var grants = await _service.GetMyCreatedGrantsAsync(userId);
        return Ok(grants);
    }

    [HttpPost("user/apply")]
    [Authorize]
    public async Task<IActionResult> ApplyToGrant([FromBody] GrantApplicationDto dto)
    {
        var result = await _service.ApplyForGrantAsync(dto, GetUserId());
        return result ? Ok() : BadRequest("Grant does not exist or is closed.");
    }

    [HttpGet("user/my-applications")]
    [Authorize]
    public async Task<IActionResult> GetMyApplications()
    {
        var apps = await _service.GetUserGrantApplicationsAsync(GetUserId());
        return Ok(apps);
    }

    [HttpGet("user/grant/all")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAvailableGrants()
    {
        var grants = await _service.GetAllAvailableGrantsAsync();
        return Ok(grants);
    }

    [HttpGet("user/grant/details/{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetGrantDetails(Guid id)
    {
        var grant = await _service.GetGrantDetailsAsync(id);
        return grant != null ? Ok(grant) : NotFound();
    }
}
