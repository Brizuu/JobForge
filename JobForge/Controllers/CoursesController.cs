using System.Security.Claims;
using JobForge.DbModels;
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

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateCourse([FromBody] CourseDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized();

        dto.CreatorId = userId;
        var created = await _service.CreateCourseAsync(dto);
        return Ok(created);
    }
    
    [HttpPut("{courseId}")]
    [Authorize]
    public async Task<IActionResult> UpdateCourse(int courseId, [FromBody] CourseDto updatedDto)
    {
        var result = await _service.UpdateCourseAsync(courseId, updatedDto);
        if (result == null)
            return NotFound("Course not found");

        return Ok(result);
    }

    [HttpDelete("{courseId}")]
    [Authorize]
    public async Task<IActionResult> DeleteCourse(int courseId)
    {
        var success = await _service.DeleteCourseAsync(courseId);
        if (!success)
            return NotFound("Course not found");

        return NoContent();
    }
    
    [HttpGet ("getAll")]
    public async Task<IActionResult> GetAllCourses([FromQuery] string? category)
    {
        var courses = await _service.GetAllCoursesAsync(category);
        return Ok(courses);
    }

    [HttpGet("{courseId}")]
    public async Task<IActionResult> GetCourseById(int courseId)
    {
        var course = await _service.GetCourseByIdAsync(courseId);
        if (course == null)
            return NotFound("Course not found");

        return Ok(course);
    }

    [HttpPost("sections/{courseId}")]
    [Authorize]
    public async Task<IActionResult> AddSection(int courseId, [FromBody] CourseSectionDto sectionDto)
    {
        var result = await _service.AddSectionAsync(courseId, sectionDto);
        return Ok(result);
    }
    
    [HttpPut("sections/{sectionId}")]
    [Authorize]
    public async Task<IActionResult> UpdateSection(int sectionId, [FromBody] CourseSectionDto updatedDto)
    {
        var result = await _service.UpdateSectionAsync(sectionId, updatedDto);
        if (result == null)
            return NotFound("Section not found");

        return Ok(result);
    }

    [HttpDelete("sections/{sectionId}")]
    [Authorize]
    public async Task<IActionResult> DeleteSection(int sectionId)
    {
        var success = await _service.DeleteSectionAsync(sectionId);
        if (!success)
            return NotFound("Section not found");

        return NoContent();
    }


    [HttpGet("mine")]
    [Authorize]
    public async Task<IActionResult> GetMyCourses()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized();

        var courses = await _service.GetCoursesByCreatorAsync(userId);
        return Ok(courses);
    }
    

}
