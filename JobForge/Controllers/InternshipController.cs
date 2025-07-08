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

        private Guid GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
                throw new UnauthorizedAccessException("Brak lub niepoprawny userId w tokenie");

            return userId;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateInternship([FromBody] IntershipDto dto)
        {
            try
            {
                var userId = GetUserIdFromToken();
                var created = await _service.CreateInternshipAsync(dto, userId);
                return CreatedAtAction(nameof(GetInternships), new { id = created.Title }, created);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInternship(Guid id)
        {
            var deleted = await _service.DeleteInternshipAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

        [HttpPost("applications/create")]
        public async Task<IActionResult> CreateApplication([FromBody] InternshipApplicationDto dto)
        {
            try
            {
                var userId = GetUserIdFromToken();
                var created = await _service.CreateApplicationAsync(dto, userId);
                return CreatedAtAction(nameof(GetApplications), new { id = created.InternshipId }, created);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    error = "InternshipNotFound",
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "ServerError",
                    message = "An unexpected error occurred."
                });
            }
        }
        
        [HttpGet("{id?}")]
        public async Task<IActionResult> GetInternships([FromQuery] Guid? id = null)
        {
            if (id.HasValue)
            {
                var internship = await _service.GetInternshipByIdAsync(id.Value);
                if (internship == null)
                    return NotFound(new { error = "NotFound", message = "Internship not found." });

                return Ok(internship);
            }

            var all = await _service.GetAllInternshipsAsync();
            return Ok(all);
        }

        
        // [HttpGet("{id}")]
        // public IActionResult GetInternship(Guid id)
        // {
        //     return Ok();
        // }


        [HttpDelete("applications/{id}")]
        public async Task<IActionResult> DeleteApplication(Guid id)
        {
            var deleted = await _service.DeleteApplicationAsync(id);
            if (!deleted) return NotFound();
            return NoContent(); 
        }
        
        [HttpGet("applications/{internshipId?}")]
        public async Task<IActionResult> GetApplications([FromQuery] Guid? internshipId = null)
        {
            var applications = await _service.GetApplicationsAsync(internshipId);
            return Ok(applications);
        }

        



        
        

        // [HttpGet("applications/{id}")]
        // public IActionResult GetApplication(Guid id)
        // {
        //     return Ok(); 
        // }
    }
}
