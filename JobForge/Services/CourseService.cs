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
            Institution = dto.Institution,
            CompletionTime = dto.CompletionTime,
            Published = dto.Published ?? false,
            Sections = new List<CourseSection>()
        };

        _context.Courses.Add(course);
        await _context.SaveChangesAsync();

        foreach (var sectionDto in dto.Sections.Where(s => s != null))
        {
            var section = new CourseSection
            {
                Title = sectionDto!.Title,
                Description = sectionDto.Description,
                Category = sectionDto.Category,
                ImageUrl = sectionDto.ImageUrl,
                VideoUrl = sectionDto.VideoUrl,
                CompletionTime = sectionDto.CompletionTime,
                CompletionPercentage = sectionDto.CompletionPercentage,
                CourseId = course.Id
            };
            _context.CourseSections.Add(section);
        }

        await _context.SaveChangesAsync();
        await _context.Entry(course).Collection(c => c.Sections).LoadAsync();

        return course;
    }

    public async Task<Course?> UpdateCourseAsync(Guid courseId, CourseDto dto)
    {
        var course = await _context.Courses
            .Include(c => c.Sections)
            .FirstOrDefaultAsync(c => c.Id == courseId);

        if (course == null)
            return null;

        course.Title = dto.Title;
        course.Description = dto.Description;
        course.Category = dto.Category;
        course.Institution = dto.Institution;
        course.CompletionTime = dto.CompletionTime;
        course.Published = dto.Published ?? course.Published;

        // Nadpisujemy sekcje
        _context.CourseSections.RemoveRange(course.Sections);
        await _context.SaveChangesAsync();

        foreach (var sectionDto in dto.Sections.Where(s => s != null))
        {
            var section = new CourseSection
            {
                Title = sectionDto!.Title,
                Description = sectionDto.Description,
                Category = sectionDto.Category,
                ImageUrl = sectionDto.ImageUrl,
                VideoUrl = sectionDto.VideoUrl,
                CompletionTime = sectionDto.CompletionTime,
                CompletionPercentage = sectionDto.CompletionPercentage,
                CourseId = course.Id
            };
            _context.CourseSections.Add(section);
        }

        await _context.SaveChangesAsync();
        return course;
    }

    public async Task<bool> DeleteCourseAsync(Guid courseId)
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

    public async Task<CourseSection> AddSectionAsync(Guid courseId, CourseSectionDto sectionDto)
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
            VideoUrl = sectionDto.VideoUrl,
            CompletionTime = sectionDto.CompletionTime,
            CompletionPercentage = sectionDto.CompletionPercentage,
            CourseId = courseId
        };

        _context.CourseSections.Add(section);
        await _context.SaveChangesAsync();
        return section;
    }

    public async Task<CourseSection?> UpdateSectionAsync(int sectionId, CourseSectionEditDto updatedDto)
    {
        var section = await _context.CourseSections.FindAsync(sectionId);
        if (section == null)
            return null;

        section.Title = updatedDto.Title ?? section.Title;
        section.Description = updatedDto.Description ?? section.Description;
        section.Category = updatedDto.Category ?? section.Category;
        section.ImageUrl = updatedDto.ImageUrl ?? section.ImageUrl;
        section.VideoUrl = updatedDto.VideoUrl ?? section.VideoUrl;
        section.CompletionTime = updatedDto.CompletionTime ?? section.CompletionTime;
        section.CompletionPercentage = updatedDto.CompletionPercentage ?? section.CompletionPercentage;

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

    public async Task<IEnumerable<object>> GetAllCoursesAsync(string? category)
    {
        var query = _context.Courses
            .Where(c => c.Published) 
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(c => c.Category.ToLower() == category.ToLower());
        }

        var result = await query
            .Select(c => new
            {
                c.Id,
                c.Title,
                c.Institution,
                c.CompletionTime,
                c.Category
            })
            .ToListAsync();

        return result;
    }

    public async Task<bool> ChangeCoursePublishedStateAsync(Guid courseId, Guid userId, bool newState)
    {
        var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == courseId && c.CreatorId == userId);
        if (course == null)
            return false;

        course.Published = newState;
        await _context.SaveChangesAsync();
        return true;
    }



    public async Task<Course?> GetCourseByIdAsync(Guid courseId)
    {
        return await _context.Courses
            .Include(c => c.Sections)
            .FirstOrDefaultAsync(c => c.Id == courseId);
    }

    public async Task<bool> StartCourseAsync(Guid userId, Guid courseId)
    {
        var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == courseId && c.Published);
        if (course == null) return false;

        var existing = await _context.UserCourses
            .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CourseId == courseId);
        if (existing != null) return true; // Already added

        // Automatyczne pobranie GeneratedCV
        var cv = await _context.GeneratedCVs.FirstOrDefaultAsync(c => c.UserId == userId);
        if (cv == null) return false; // Nie znaleziono CV — można też rozważyć stworzenie nowego

        var userCourse = new UserCourse
        {
            CourseId = courseId,
            UserId = userId,
            GeneratedCVId = cv.Id,
            Title = course.Title,
            Description = course.Description,
            Institution = course.Institution,
            CompletionTime = course.CompletionTime,
            Category = course.Category,
            isCompleted = false,
            Verified = true,
            CompletionPercentage = 0
        };

        _context.UserCourses.Add(userCourse);
        await _context.SaveChangesAsync();

        return true;
    }



    public async Task UpdateCompletionPercentageAsync(Guid userId, Guid courseId, double newPercentage)
    {
        var userCourse = await _context.UserCourses
            .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CourseId == courseId);

        if (userCourse != null)
        {
            userCourse.CompletionPercentage = newPercentage;
            await _context.SaveChangesAsync();
        }
    }
    
    public async Task<CourseSection?> GetNextSectionAsync(Guid userId, Guid courseId)
    {
        var userCourse = await _context.UserCourses
            .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CourseId == courseId);

        if (userCourse == null)
            return null;

        var sections = await _context.CourseSections
            .Where(s => s.CourseId == courseId)
            .OrderBy(s => s.Id)
            .ToListAsync();

        if (!sections.Any())
            return null;

        int? lastCompleted = userCourse.LastCompletedSectionId;
        CourseSection? nextSection;

        if (lastCompleted == null || lastCompleted == 0)
        {
            nextSection = sections.FirstOrDefault();
        }
        else
        {
            nextSection = sections.FirstOrDefault(s => s.Id > lastCompleted);
        }

        if (nextSection == null)
            return null;
        
        userCourse.LastCompletedSectionId = nextSection.Id;
        
        var index = sections.IndexOf(nextSection) + 1; // +1 because we're completing this one
        var total = sections.Count;
        userCourse.CompletionPercentage = Math.Round((double)index / total * 100, 2);

        await _context.SaveChangesAsync();

        return nextSection;
    }



    public async Task<IEnumerable<UserCourseDto>> GetCoursesByUserIdAsync(Guid userId)
    {
        return await _context.UserCourses
            .Where(uc => uc.UserId == userId)
            .Select(uc => new UserCourseDto
            {
                Title = uc.Title,
                Description = uc.Description,
                Institution = uc.Institution,
                CompletionTime = uc.CompletionTime,
                Category = uc.Category,
                CompletionPercentage = uc.CompletionPercentage
            })
            .ToListAsync();
    }
}
