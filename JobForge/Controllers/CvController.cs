using System.Security.Claims;
using JobForge.DbModels;
using JobForge.Models;
using JobForge.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace JobForge.Controllers;

[Authorize(Roles = "Admin, Premium")]
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
    
    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            throw new UnauthorizedAccessException("Brak identyfikatora użytkownika.");
        return Guid.Parse(userIdClaim.Value);
    }
    
    
    [HttpPost("personal-information")]
    public async Task<IActionResult> Add(PersonalInformationDto dto)
    {
        await _service.AddPersonalInformations(GetUserId(), dto);
        return Ok();
    }

    [HttpGet("personal-information")]
    public async Task<ActionResult<PersonalInformationDto?>> Get()
    {
        var result = await _service.GetPersonalInformations(GetUserId());
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPut("personal-information")]
    public async Task<IActionResult> Update(PersonalInformationEditDto dto)
    {
        await _service.UpdatePersonalInformations(GetUserId(), dto);
        return NoContent();
    }
    
    
    
    [HttpPost("work-experience")]
    public async Task<IActionResult> Add(WorkExperienceDto dto)
    {
        try
        {
            await _service.AddWorkExperience(GetUserId(), dto);
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpGet("work-experience")]
    public async Task<IActionResult> GetAll()
    {
        var userId = GetUserId();
        if (userId == Guid.Empty)
            return Unauthorized();

        var result = await _service.GetWorkExperienceAsync(userId);
        return Ok(result);
    }


    [HttpPut("work-experience/{id:guid}")]
    public async Task<IActionResult> UpdateWorkExperience(Guid id, [FromBody] WorkExperienceEditDto dto)
    {
        var userId = GetUserId(); 
        var updated = await _service.UpdateWorkExperience(userId, id, dto);

        if (!updated)
            return NotFound("Nie znaleziono doświadczenia zawodowego o podanym Id.");

        return Ok("Doświadczenie zawodowe zostało zaktualizowane.");
    }
    
    [HttpDelete("work-experience/{id:guid}")]
    public async Task<IActionResult> DeleteWorkExperience(Guid id)
    {
        var userId = GetUserId();
        await _service.DeleteWorkExperience(userId, id);
        return NoContent();
    }
    
    
    [HttpPost("language")]
    public async Task<IActionResult> AddLanguage([FromBody] LanguageDto dto)
    {
        try
        {
            await _service.AddLanguage(GetUserId(), dto);
            return Ok("Język został dodany.");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("language")]
    public async Task<IActionResult> GetLanguages()
    {
        var userId = GetUserId(); 
        var result = await _service.GetUserLanguages(userId);
        return Ok(result);
    }

    [HttpPut("language/{id:guid}")]
    public async Task<IActionResult> UpdateLanguage(Guid id, [FromBody] LanguageEditDto dto)
    {
        var success = await _service.UpdateLanguage(GetUserId(), id, dto);
        return success ? Ok("Zaktualizowano język.") : NotFound("Nie znaleziono języka.");
    }
    
    [HttpDelete("language/{id:guid}")]
    public async Task<IActionResult> DeleteLanguage(Guid id)
    {
        var userId = GetUserId();
        await _service.DeleteLanguage(userId, id);
        return NoContent();
    }

    
    
    [HttpGet("soft-skills")]
    public async Task<IActionResult> GetSoftSkills()
    {
        var userId = GetUserId(); 
        var skills = await _service.GetSoftSkills(userId);
        return Ok(skills);
    }

    [HttpPost("soft-skills")]
    public async Task<IActionResult> AddSoftSkill([FromBody] SoftSkillsDto dto)
    {
        var userId = GetUserId();
        var skill = await _service.AddSoftSkills(userId, dto);
        return CreatedAtAction(nameof(GetSoftSkills), new { id = skill.Id }, skill);
    }
    
    [HttpPut("soft-skills/{id:guid}")]
    public async Task<IActionResult> UpdateSoftSkill(Guid id, [FromBody] SoftSkillsEditDto dto)
    {
        var userId = GetUserId();
        var updated = await _service.UpdateSoftSkills(userId, id, dto);
        if (!updated)
            return NotFound("Nie znaleziono umiejętności miękkiej o podanym Id.");

        return Ok("Umiejętność miękka została zaktualizowana.");
    }
    
    [HttpDelete("soft-skills/{id:guid}")]
    public async Task<IActionResult> DeleteSoftSkills(Guid id)
    {
        var userId = GetUserId();
        await _service.DeleteSoftSkills(userId, id);
        return NoContent();
    }
    
    
    
    [HttpGet("technical-skills")]
    public async Task<IActionResult> GetTechnicalSkills()
    {
        var userId = GetUserId();
        var skills = await _service.GetTechnicalSkills(userId);
        return Ok(skills);
    }
    
    [HttpPost("technical-skills")]
    public async Task<IActionResult> AddTechnicalSkill([FromBody] TechnicalSkillsDto dto)
    {
        var userId = GetUserId();
        var skill = await _service.AddTechnicalSkill(userId, dto);
        return CreatedAtAction(nameof(GetTechnicalSkills), new { id = skill.Id }, skill);
    }
    
    [HttpPut("technical-skills/{id:guid}")]
    public async Task<IActionResult> UpdateTechnicalSkill(Guid id, [FromBody] TechnicalSkillsEditDto dto)
    {
        var userId = GetUserId();
        var updated = await _service.UpdateTechnicalSkill(userId, id, dto);
        if (!updated)
            return NotFound("Nie znaleziono umiejętności technicznej o podanym Id.");

        return Ok("Umiejętność techniczna została zaktualizowana.");
    }
    
    [HttpDelete("technical-skills/{id:guid}")]
    public async Task<IActionResult> DeleteTechnicalSkill(Guid id)
    {
        var userId = GetUserId();
        await _service.DeleteTechnicalSkill(userId, id);
        return NoContent();
    }

    
    
    [HttpGet("interests")]
    public async Task<IActionResult> GetInterests()
    {
        var userId = GetUserId();
        var interests = await _service.GetInterests(userId);
        return Ok(interests);
    }
    
    [HttpPost("interests")]
    public async Task<IActionResult> AddInterest([FromBody] InterestsDto dto)
    {
        var userId = GetUserId();
        var interest = await _service.AddInterest(userId, dto);
        return CreatedAtAction(nameof(GetInterests), new { id = interest.Id }, interest);
    }
    
    [HttpPut("interests/{id:guid}")]
    public async Task<IActionResult> UpdateInterest(Guid id, [FromBody] InterestsEditDto dto)
    {
        var userId = GetUserId();
        var updated = await _service.UpdateInterest(userId, id, dto);
        if (!updated)
            return NotFound("Nie znaleziono zainteresowania o podanym Id.");

        return Ok("Zainteresowanie zostało zaktualizowane.");
    }
    
    [HttpDelete("interests/{id:guid}")]
    public async Task<IActionResult> DeleteInterest(Guid id)
    {
        var userId = GetUserId();
        await _service.DeleteInterest(userId, id);
        return NoContent();
    }

    
    
    [HttpPost("user-courses")]
    public async Task<IActionResult> AddCourse([FromBody] UserCourseDto dto)
    {
        var userId = GetUserId();
        await _service.AddUserCourse(userId, dto);
        return Ok();
    }

    [HttpGet("user-courses")]
    public async Task<IActionResult> GetCourses()
    {
        var userId = GetUserId();
        var result = await _service.GetUserCourses(userId);
        return Ok(result);
    }

    [HttpPut("user-courses/{courseId:guid}")]
    public async Task<IActionResult> Update(Guid courseId, [FromBody] UserCourseEditDto dto)
    {
        var userId = GetUserId();
        await _service.UpdateUserCourse(userId, courseId, dto);
        return NoContent();
    }
    
    [HttpDelete("user-courses/{courseId:guid}")]
    public async Task<IActionResult> Delete(Guid courseId)
    {
        var userId = GetUserId();
        await _service.DeleteUserCourse(userId, courseId);
        return NoContent();
    }
    
    
    [HttpPost("generate-cv")]
    public async Task<IActionResult> GenerateCv()
    {
        var userId = GetUserId();
        var generatedCv = await _service.GenerateCvAsync(userId);
        return Ok(generatedCv);
    }

    [HttpGet("get-cv")]
    public async Task<IActionResult> GetGeneratedCv()
    {
        var userId = GetUserId();
        var cvDto = await _service.GetGeneratedCvAsync(userId);
        if (cvDto == null) return NotFound();
        return Ok(cvDto);
    }

}
