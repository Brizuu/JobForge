using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using JobForge.Services;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RaportController : ControllerBase
{
    private readonly IRaportService _raportService;

    public RaportController(IRaportService raportService)
    {
        _raportService = raportService;
    }

    [HttpGet("count-registered/{companyId}")]
    public async Task<IActionResult> CountUsersInCompany(Guid companyId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("Brak identyfikatora użytkownika w tokenie.");
        }

        var count = await _raportService.CountUsersInCompanyAsync(companyId);
        return Ok(new { companyId, userId, userCount = count });
    }
}