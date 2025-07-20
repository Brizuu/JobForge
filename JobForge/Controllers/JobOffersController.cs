using System.Security.Claims;
using JobForge.DbModels;
using JobForge.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobForge.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobOffersController : ControllerBase
{
    private readonly IJobOfferService _jobOfferService;

    public JobOffersController(IJobOfferService jobOfferService)
    {
        _jobOfferService = jobOfferService;
    }

    [HttpPost("company/create")]
    [Authorize]
    public async Task<IActionResult> AddJobOffer([FromBody] JobOfferDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized("User ID not found in token.");
        }

        var success = await _jobOfferService.AddJobOfferAsync(dto, userId);
        if (!success)
            return BadRequest("Could not add job offer.");

        return Ok("Job offer added successfully.");
    }
    
    [HttpPatch("company/archive/{id}")]
    public async Task<IActionResult> ArchiveJobOffer(int id, [FromQuery] bool archive)
    {
        var result = await _jobOfferService.ArchiveJobOfferAsync(id, archive);

        if (!result)
            return NotFound(new { message = "Job offer not found or operation failed." });

        return Ok(new { message = archive ? "Job offer archived." : "Job offer restored." });
    }
    
    
    [Authorize]
    [HttpPatch("company/application/status/{applicationId}")]
    public async Task<IActionResult> UpdateApplicationStatus(int applicationId, [FromBody] string newStatus)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out Guid userId))
            return Unauthorized();

        var result = await _jobOfferService.ReviewApplicationAsync(applicationId, newStatus, userId);

        if (!result)
            return Forbid("Brak dostępu lub aplikacja/oferta nie istnieje.");

        return Ok("Status aplikacji został zaktualizowany.");
    }

    
    [HttpGet("company/applications/{jobOfferId}")]
    public async Task<IActionResult> GetFullApplicationsForOffer(int jobOfferId)
    {
        
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var employerId))
        {
            return Unauthorized("User ID not found in token.");
        }
        

        var applications = await _jobOfferService.GetApplicationsWithCVsForOfferAsync(jobOfferId, employerId);

        if (applications == null || applications.Count == 0)
            return NotFound("Brak aplikacji lub nie jesteś właścicielem oferty.");

        return Ok(applications);
    }




    
    [HttpPost("user/apply/{id}")]
    [Authorize]
    public async Task<IActionResult> ApplyToJobOffer([FromRoute] int id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
        {
            return Unauthorized("Invalid user token.");
        }

        var (success, message) = await _jobOfferService.ApplyToJobOfferAsync(id, userId);

        if (!success)
            return BadRequest(message);

        return Ok(message);
    }
    
    [HttpGet("user/get-offers")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllJobOffers()
    {
        var offers = await _jobOfferService.GetAllJobOffersAsync();
        return Ok(offers);
    }

    [HttpGet("user/get-details/{id}")]
    public async Task<IActionResult> GetJobOfferDetails(int id)
    {
        var offer = await _jobOfferService.GetJobOfferByIdAsync(id);

        if (offer == null)
            return NotFound();

        return Ok(offer);
    }
    
    [HttpPost("user/add-favourite")]
    public async Task<IActionResult> AddFavorite([FromBody] FavoriteJobOfferDto dto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _jobOfferService.AddFavoriteAsync(dto.JobOfferId, userId);
        return Ok();
    }
    
    [HttpGet("user/get-favourites")]
    public async Task<IActionResult> GetFavorites()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var favorites = await _jobOfferService.GetFavoritesByUserAsync(userId);
        return Ok(favorites);
    }
    
    [HttpGet("user/my-applications")]
    [Authorize]
    public async Task<IActionResult> GetMyApplications()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var applications = await _jobOfferService.GetUserJobApplicationsAsync(userId);
        return Ok(applications);
    }


}
