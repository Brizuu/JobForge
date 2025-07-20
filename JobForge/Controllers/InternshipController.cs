using JobForge.DbModels;
using JobForge.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace JobForge.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]  
    public class InternshipController : ControllerBase
    {
        private readonly IInternshipService _service;

        public InternshipController(IInternshipService service)
        {
            _service = service;
        }

        private Guid GetUserId() => Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

  
    [HttpPost("company/internship/create")]
    [Authorize(Roles = "Company, PublicFacility, Admin")]
    public async Task<IActionResult> CreateInternship([FromBody] InternshipDto dto)
    {
        var id = await _service.CreateInternshipAsync(dto, GetUserId());
        return Ok(new { InternshipId = id });
    }

   
    [HttpPut("company/internship/{id}")]
    [Authorize(Roles = "Company, PublicFacility, Admin")]
    public async Task<IActionResult> UpdateInternship(Guid id, [FromBody] InternshipDto dto)
    {
        var result = await _service.UpdateInternshipAsync(id, dto, GetUserId());
        return result ? Ok() : Forbid();
    }


    [HttpPatch("company/internship/archive-toggle/{id}")]
    [Authorize(Roles = "Company, PublicFacility, Admin")]
    public async Task<IActionResult> ToggleArchive(Guid id)
    {
        var result = await _service.ToggleArchiveStatusAsync(id, GetUserId());
        return result ? Ok() : Forbid();
    }


    [HttpGet("company/internship/my")]
    [Authorize(Roles = "Company, PublicFacility, Admin")]
    public async Task<IActionResult> GetOwnInternships()
    {
        var internships = await _service.GetInternshipsByAuthorAsync(GetUserId());
        return Ok(internships);
    }


    [HttpGet("company/internship/applications/{internshipId}")]
    [Authorize(Roles = "Company, PublicFacility, Admin")]
    public async Task<IActionResult> GetApplications(Guid internshipId)
    {
        var apps = await _service.GetApplicationsForInternshipAsync(internshipId, GetUserId());
        return Ok(apps);
    }

 
    [HttpPatch("applications/{applicationId}/review")]
    [Authorize(Roles = "Company, PublicFacility, Admin")]
    public async Task<IActionResult> ReviewApplication(Guid applicationId, [FromQuery] string status)
    {
        var result = await _service.ReviewInternshipApplicationAsync(applicationId, status, GetUserId());
        return result ? Ok(new { Message = "Status updated." }) : Forbid();
    }

    
    [HttpPost("user/internship/apply")]
    [Authorize]
    public async Task<IActionResult> ApplyToInternship([FromBody] InternshipApplicationDto dto)
    {
        var result = await _service.ApplyForInternshipAsync(dto, GetUserId());
        return result ? Ok(new { Message = "Application submitted." }) : BadRequest("Internship not found or archived.");
    }


    [HttpGet("user/internship/my-applications")]
    [Authorize]
    public async Task<IActionResult> GetUserApplications()
    {
        var apps = await _service.GetUserInternshipApplicationsAsync(GetUserId());
        return Ok(apps);
    }

    
    [HttpGet("user/internship/all")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAvailableInternships()
    {
        var internships = await _service.GetAllAvailableInternshipsAsync();
        return Ok(internships);
    }

 
    [HttpGet("user/internship/details/{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetInternshipDetails(Guid id)
    {
        var internship = await _service.GetInternshipDetailsAsync(id);
        return internship != null ? Ok(internship) : NotFound();
    }
    }
}
