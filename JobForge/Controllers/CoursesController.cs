using System.Security.Claims;
using JobForge.DbModels;
using JobForge.Models;
using JobForge.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobForge.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _service;

    public CoursesController(ICourseService service)
    {
        _service = service;
    }

    [HttpPost("creator")]
    [Authorize]
    public async Task<IActionResult> CreateCourse([FromBody] CourseDto courseDto)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized("Invalid user ID");

        var course = await _service.CreateCourseAsync(userId.Value, courseDto);
        return CreatedAtAction(nameof(GetCourseById), new { courseId = course.Id }, course);
    }

    [HttpPut("creator/{courseId:guid}")]
    [Authorize]
    public async Task<IActionResult> UpdateCourse(Guid courseId, [FromBody] CourseEditDto updatedDto)
    {
        var result = await _service.UpdateCourseAsync(courseId, new CourseDto
        {
            Title = updatedDto.Title,
            Description = updatedDto.Description,
            Category = updatedDto.Category,
            Institution = updatedDto.Institution,
            CompletionTime = updatedDto.CompletionTime,
            Sections = updatedDto.Sections
        });

        if (result == null)
            return NotFound("Course not found");

        return Ok(result);
    }

    [HttpDelete("creator/{courseId:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteCourse(Guid courseId)
    {
        var success = await _service.DeleteCourseAsync(courseId);
        return success ? NoContent() : NotFound("Course not found");
    }

    [HttpPost("creator/sections/{courseId}")]
    [Authorize]
    public async Task<IActionResult> AddSection(Guid courseId, [FromBody] CourseSectionDto sectionDto)
    {
        var result = await _service.AddSectionAsync(courseId, sectionDto);
        return Ok(result);
    }

    [HttpPut("creator/sections/{sectionId}")]
    [Authorize]
    public async Task<IActionResult> UpdateSection(int sectionId, [FromBody] CourseSectionEditDto updatedDto)
    {
        var result = await _service.UpdateSectionAsync(sectionId, updatedDto);
        return result == null ? NotFound("Section not found") : Ok(result);
    }

    [HttpDelete("creator/sections/{sectionId}")]
    [Authorize]
    public async Task<IActionResult> DeleteSection(int sectionId)
    {
        var success = await _service.DeleteSectionAsync(sectionId);
        return success ? NoContent() : NotFound("Section not found");
    }

    [HttpGet("creator/created-courses")]
    [Authorize]
    public async Task<IActionResult> GetMyCourses()
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized();

        var courses = await _service.GetCoursesByCreatorAsync(userId.Value);
        return Ok(courses);
    }
    
    [HttpPut("creator/publish/{courseId:guid}")]
    public async Task<IActionResult> ChangeCoursePublishedState(Guid courseId, [FromQuery] bool published)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized();

        var success = await _service.ChangeCoursePublishedStateAsync(courseId, userId, published);
        if (!success)
            return NotFound("Course not found or you're not the creator.");

        return Ok($"Course {(published ? "published" : "unpublished")} successfully.");
    }

    
    [HttpGet("user/get-all-courses")]
    public async Task<IActionResult> GetAllCourses([FromQuery] string? category)
    {
        var courses = await _service.GetAllCoursesAsync(category);
        return Ok(courses);
    }

    [HttpGet("user/get-details/{courseId:guid}")]
    public async Task<IActionResult> GetCourseById(Guid courseId)
    {
        var course = await _service.GetCourseByIdAsync(courseId);
        return course == null ? NotFound("Course not found") : Ok(course);
    }
    
    [HttpPost("user/start-course")]
    public async Task<IActionResult> StartCourse([FromQuery] Guid courseId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized();

        var success = await _service.StartCourseAsync(userId, courseId);
        if (!success)
            return BadRequest("Cannot start the course — either it doesn't exist, isn't published, or no CV is available.");

        return Ok("Course added to your profile.");
    }


    
    [HttpPost("user/next-section/{courseId:guid}")]
    public async Task<IActionResult> GetNextSection(Guid courseId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized();

        var section = await _service.GetNextSectionAsync(userId, courseId);
        if (section == null)
            return BadRequest("No more sections available or course not found.");

        return Ok(section);
    }


    // [HttpPut("user/update-percentage")]
    // [Authorize]
    // public async Task<IActionResult> UpdateCompletionPercentage([FromQuery] Guid courseId, [FromQuery] double percentage)
    // {
    //     var userId = GetUserId();
    //     if (userId == null)
    //         return Unauthorized();
    //
    //     await _service.UpdateCompletionPercentageAsync(userId.Value, courseId, percentage);
    //     return Ok("Completion percentage updated.");
    // }

    [HttpGet("user/my-courses")]
    [Authorize]
    public async Task<IActionResult> GetUserCourses()
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized();

        var courses = await _service.GetCoursesByUserIdAsync(userId.Value);
        return Ok(courses);
    }

    private Guid? GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
        return Guid.TryParse(claim?.Value, out var userId) ? userId : null;
    }
}
