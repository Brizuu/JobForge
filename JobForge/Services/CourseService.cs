using JobForge.Data;
using JobForge.DbModels;
using Microsoft.EntityFrameworkCore;

namespace JobForge.Services;

public class CourseService : ICourseService
{
    private readonly AppDbContext _context;

    public CourseService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CourseDto> CreateCourseAsync(CourseDto dto)
    {
        _context.Courses.Add(dto);
        await _context.SaveChangesAsync();
        return dto;
    }
    
    public async Task<CourseDto?> UpdateCourseAsync(int courseId, CourseDto updated)
    {
        var course = await _context.Courses.FindAsync(courseId);
        if (course == null)
            return null;

        course.Title = updated.Title;
        course.Description = updated.Description;

        await _context.SaveChangesAsync();
        return course;
    }

    public async Task<bool> DeleteCourseAsync(int courseId)
    {
        var course = await _context.Courses
            .Include(c => c.Sections)
            .FirstOrDefaultAsync(c => c.Id == courseId);

        if (course == null)
            return false;

        _context.CourseSections.RemoveRange(course.Sections);
        _context.Courses.Remove(course);

        await _context.SaveChangesAsync();
        return true;
    }


    public async Task<CourseSectionDto> AddSectionAsync(int courseId, CourseSectionDto section)
    {
        var course = await _context.Courses.FindAsync(courseId);
        if (course == null)
            throw new Exception("Course not found");

        section.CourseId = courseId;
        _context.CourseSections.Add(section);
        await _context.SaveChangesAsync();
        return section;
    }
    
    public async Task<CourseSectionDto?> UpdateSectionAsync(int sectionId, CourseSectionDto updated)
    {
        var section = await _context.CourseSections.FindAsync(sectionId);
        if (section == null)
            return null;

        section.Title = updated.Title;
        section.Description = updated.Description;
        section.Category = updated.Category;
        section.ImageUrl = updated.ImageUrl;
        section.CompletionPercentage = updated.CompletionPercentage;

        await _context.SaveChangesAsync();
        return section;
    }

    public async Task<bool> DeleteSectionAsync(int sectionId)
    {
        var section = await _context.CourseSections.FindAsync(sectionId);
        if (section == null)
            return false;

        _context.CourseSections.Remove(section);
        await _context.SaveChangesAsync();
        return true;
    }


    public async Task<List<CourseDto>> GetCoursesByCreatorAsync(Guid creatorId)
    {
        return await _context.Courses
            .Include(c => c.Sections)
            .Where(c => c.CreatorId == creatorId)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<CourseDto>> GetAllCoursesAsync(string? category)
    {
        var query = _context.Courses
            .Include(c => c.Sections)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(c => c.Category.ToLower() == category.ToLower());
        }

        return await query.ToListAsync();
    }

    public async Task<CourseDto?> GetCourseByIdAsync(int courseId)
    {
        var course = await _context.Courses
            .Include(c => c.Sections)
            .FirstOrDefaultAsync(c => c.Id == courseId);

        return course;
    }

}
