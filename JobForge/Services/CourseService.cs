using JobForge.Data;
using JobForge.DbModels;
using JobForge.Models;
using Microsoft.EntityFrameworkCore;

namespace JobForge.Services;

public class CourseService : ICourseService
{
    private readonly AppDbContext _context;

    public CourseService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Course> CreateCourseAsync(Guid userId, CourseDto dto)
    {
        var course = new Course
        {
            Title = dto.Title,
            Description = dto.Description,
            Category = dto.Category,
            CreatorId = userId,
            Sections = new List<CourseSection>() // Można zostawić pustą na start
        };

        _context.Courses.Add(course);
        await _context.SaveChangesAsync();

        foreach (var sectionDto in dto.Sections)
        {
            var section = new CourseSection
            {
                Title = sectionDto.Title,
                Description = sectionDto.Description,
                Category = sectionDto.Category,
                ImageUrl = sectionDto.ImageUrl,
                CompletionPercentage = sectionDto.CompletionPercentage,
                CourseId = course.Id
            };
            _context.CourseSections.Add(section);
            // UWAGA: Nie dodajemy tutaj do course.Sections, żeby nie mieć duplikatów
        }

        await _context.SaveChangesAsync();

        // Załaduj sekcje, aby mieć aktualną kolekcję
        await _context.Entry(course).Collection(c => c.Sections).LoadAsync();

        return course;
    }


    
    public async Task<Course?> UpdateCourseAsync(int courseId, CourseDto dto)
    {
        var course = await _context.Courses.FindAsync(courseId);
        if (course == null)
            return null;

        course.Title = dto.Title;
        course.Description = dto.Description;
        course.Category = dto.Category;  // zakładam, że Category jest w dto
        

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


    public async Task<CourseSection> AddSectionAsync(int courseId, CourseSectionDto sectionDto)
    {
        var course = await _context.Courses.FindAsync(courseId);
        if (course == null)
            throw new Exception("Course not found");

        var section = new CourseSection
        {
            Title = sectionDto.Title,
            Description = sectionDto.Description,
            Category = sectionDto.Category,
            ImageUrl = sectionDto.ImageUrl,
            CompletionPercentage = sectionDto.CompletionPercentage,
            CourseId = courseId
        };

        _context.CourseSections.Add(section);
        await _context.SaveChangesAsync();
        return section;
    }

    
    public async Task<CourseSection?> UpdateSectionAsync(int sectionId, CourseSectionDto updatedDto)
    {
        var section = await _context.CourseSections.FindAsync(sectionId);
        if (section == null)
            return null;

        section.Title = updatedDto.Title;
        section.Description = updatedDto.Description;
        section.Category = updatedDto.Category;
        section.ImageUrl = updatedDto.ImageUrl;
        section.CompletionPercentage = updatedDto.CompletionPercentage;

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


    public async Task<List<Course>> GetCoursesByCreatorAsync(Guid creatorId)
    {
        return await _context.Courses
            .Include(c => c.Sections)
            .Where(c => c.CreatorId == creatorId)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Course>> GetAllCoursesAsync(string? category)
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

    public async Task<Course?> GetCourseByIdAsync(int courseId)
    {
        var course = await _context.Courses
            .Include(c => c.Sections)
            .FirstOrDefaultAsync(c => c.Id == courseId);

        return course;
    }
    
    public async Task AddUserCourseAsync(Guid userId, UserCourseCreateDto dto)
    {
        var entity = new UserCourse
        {
            UserId = userId,
            CourseId = dto.CourseId,
            CourseTitle = dto.CourseTitle,
            CourseDescription = dto.CourseDescription,
            CompletionPercentage = dto.CompletionPercentage
        };

        _context.UserCourses.Add(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateCompletionPercentageAsync(Guid userId, UserCourseUpdateDto dto)
    {
        var userCourse = await _context.UserCourses
            .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CourseId == dto.CourseId);

        if (userCourse != null)
        {
            userCourse.CompletionPercentage = dto.CompletionPercentage;
            await _context.SaveChangesAsync();
        }
    }

    
    public async Task<IEnumerable<UserCourseDto>> GetCoursesByUserIdAsync(Guid userId)
    {
        return await _context.UserCourses
            .Where(uc => uc.UserId == userId)
            .Select(uc => new UserCourseDto
            {
                CourseId = uc.CourseId,
                CourseTitle = uc.CourseTitle,
                CourseDescription = uc.CourseDescription,
                CompletionPercentage = uc.CompletionPercentage
            })
            .ToListAsync();
    }

    public async Task AddCompletedCourseTitleAsync(Guid userId, int courseId)
    {
        var course = await _context.UserCourses
            .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CourseId == courseId);

        if (course == null || course.CompletionPercentage < 100)
            return; 

        var personalInfo = await _context.PersonalInformations
            .FirstOrDefaultAsync(pi => pi.UserId == userId);

        if (personalInfo == null)
            return; 

        if (!personalInfo.Courses.Contains(course.CourseTitle))
        {
            personalInfo.Courses.Add(course.CourseTitle);
            await _context.SaveChangesAsync();
        }
    }

}
