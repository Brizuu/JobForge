using System.Security.Claims;
using JobForge.DbModels;
using JobForge.Models;
using JobForge.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace JobForge.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CvController : ControllerBase
{
    private readonly ICvService _service;

    public CvController(ICvService service)
    {
        _service = service;
    }
    
    private Guid GetUserIdFromToken()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            throw new UnauthorizedAccessException("Brak identyfikatora użytkownika.");
        return Guid.Parse(userIdClaim.Value);
    }
    
    [HttpPost("personal-data")]
    public async Task<IActionResult> AddPersonalData([FromBody] PersonalDataDto dto)
    {
        var userId = GetUserIdFromToken();
        var created = await _service.AddPersonalDataAsync(dto, userId);
        return CreatedAtAction(null, new { id = created.Id }, created); // Brak GET więc null
    }

    [HttpPut("personal-data/{id}")]
    public async Task<IActionResult> UpdatePersonalData(int id, [FromBody] PersonalDataDto dto)
    {
        var userId = GetUserIdFromToken();

        try
        {
            var updated = await _service.UpdatePersonalDataAsync(id, dto, userId);
            return Ok(updated);
        }
        catch (InvalidOperationException)
        {
            return NotFound(new { message = "Dane osobowe nie znalezione lub brak dostępu." });
        }
    }

    [HttpDelete("personal-data/{id}")]
    public async Task<IActionResult> DeletePersonalData(int id)
    {
        var userId = GetUserIdFromToken();

        try
        {
            await _service.DeletePersonalDataAsync(id, userId);
            return Ok(new { message = "Dane osobowe usunięte pomyślnie." });
        }
        catch (InvalidOperationException)
        {
            return NotFound(new { message = "Dane osobowe nie znalezione lub brak dostępu." });
        }
    }
    
    [HttpPost("generate")]
    public async Task<IActionResult> GenerateCv()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Unauthorized();

        Guid userId = Guid.Parse(userIdClaim.Value);

        // Sprawdzenie roli Premium
        if (!User.IsInRole("Premium"))
        {
            return Forbid("Tylko użytkownicy z rangą Premium mogą generować CV.");
        }

        try
        {
            await _service.GenerateCvAsync(userId);
            return Ok(new { message = "CV zostało pomyślnie wygenerowane." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Wystąpił błąd serwera.", error = ex.Message });
        }
    }
    
    // -------------------- WorkExperience --------------------
    [HttpPost("work-experience")]
    public async Task<IActionResult> AddWorkExperience([FromBody] WorkExperienceDto experience)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized();

        Guid userId = Guid.Parse(userIdClaim.Value);
        await _service.AddWorkExperienceAsync(experience, userId);

        return Ok(new { message = "Work experience added successfully" });
    }
    
    [HttpPut("work-experience/{id}")]
    public async Task<IActionResult> UpdateWorkExperience(int id, [FromBody] WorkExperienceDto experience)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized();

        Guid userId = Guid.Parse(userIdClaim.Value);
        await _service.UpdateWorkExperienceAsync(id, experience, userId);

        return Ok(new { message = "Work experience updated successfully" });
    }

    [HttpDelete("work-experience/{id}")]
    public async Task<IActionResult> DeleteWorkExperience(int id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized();

        Guid userId = Guid.Parse(userIdClaim.Value);
        await _service.RemoveWorkExperienceAsync(id, userId);

        return Ok(new { message = "Work experience deleted successfully" });
    }

    // -------------------- Education --------------------
    [HttpPost("education")]
    public async Task<IActionResult> AddEducation([FromBody] EducationDto education)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized();

        Guid userId = Guid.Parse(userIdClaim.Value);
        await _service.AddEducationAsync(education, userId);

        return Ok(new { message = "Education added successfully" });
    }
    
    [HttpPut("education/{id}")]
    public async Task<IActionResult> UpdateEducation(int id, [FromBody] EducationDto education)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized();

        Guid userId = Guid.Parse(userIdClaim.Value);
        await _service.UpdateEducationAsync(id, education, userId);

        return Ok(new { message = "Education updated successfully" });
    }

    [HttpDelete("education/{id}")]
    public async Task<IActionResult> DeleteEducation(int id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized();

        Guid userId = Guid.Parse(userIdClaim.Value);
        await _service.RemoveEducationAsync(id, userId);

        return Ok(new { message = "Education deleted successfully" });
    }

    // -------------------- Language --------------------

    [HttpPost("language")]
    public async Task<IActionResult> AddLanguage([FromBody] LanguageDto language)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized();

        Guid userId = Guid.Parse(userIdClaim.Value);
        await _service.AddLanguageAsync(language, userId);

        return Ok(new { message = "Language added successfully" });
    }
    
    [HttpPut("language/{id}")]
    public async Task<IActionResult> UpdateLanguage(int id, [FromBody] LanguageDto language)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized();

        Guid userId = Guid.Parse(userIdClaim.Value);
        await _service.UpdateLanguageAsync(id, language, userId);

        return Ok(new { message = "Language updated successfully" });
    }
    
    [HttpDelete("language/{id}")]
    public async Task<IActionResult> DeleteLanguage(int id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized();

        Guid userId = Guid.Parse(userIdClaim.Value);
        await _service.RemoveLanguageAsync(id, userId);

        return Ok(new { message = "Language deleted successfully" });
    }
}
