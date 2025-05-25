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

    [HttpPost]
    public async Task<IActionResult> CreateJobOffer([FromBody] JobOfferDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized();
        
        Guid userId = Guid.Parse(userIdClaim.Value);
        

        dto.UserId = userId;
        
        var id = await _jobOfferService.CreateJobOfferAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id }, null);
    }
    
    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var jobOffer = await _jobOfferService.GetJobOfferByIdAsync(id);
        if (jobOffer == null)
            return NotFound();

        return Ok(jobOffer);
    }
    
    [HttpPut("archive")]
    public async Task<IActionResult> ArchiveJobOffer([FromBody] ArchiveJobOfferDto dto)
    {
        var result = await _jobOfferService.ArchiveJobOfferAsync(dto.JobOfferId, dto.IsArchived);
        if (!result)
            return NotFound();

        return NoContent();
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteJobOffer(int id)
    {
        var result = await _jobOfferService.DeleteJobOfferAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }

    
    [HttpPost("apply")]
    [Authorize]
    public async Task<IActionResult> Apply([FromBody] ApplyToJobOfferDto dto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        try
        {
            var application = await _jobOfferService.ApplyToJobOfferAsync(dto, userId);
            return Ok(application);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    
    [HttpPost("addFavorites")]
    public async Task<IActionResult> AddFavorite([FromBody] FavoriteJobOfferDto dto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _jobOfferService.AddFavoriteAsync(dto.JobOfferId, userId);
        return Ok();
    }

    [HttpGet("getFavorites")]
    public async Task<IActionResult> GetFavorites()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var favorites = await _jobOfferService.GetFavoritesByUserAsync(userId);
        return Ok(favorites);
    }


}
