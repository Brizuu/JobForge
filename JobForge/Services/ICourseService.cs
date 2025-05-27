using JobForge.DbModels;

namespace JobForge.Services;

public interface ICourseService
{
    Task<CourseDto> CreateCourseAsync(CourseDto dto);
    
    Task<CourseDto?> UpdateCourseAsync(int courseId, CourseDto updated);
    Task<bool> DeleteCourseAsync(int courseId);
    
    Task<List<CourseDto>> GetCoursesByCreatorAsync(Guid creatorId);
    
    Task<CourseSectionDto> AddSectionAsync(int courseId, CourseSectionDto section);
    Task<CourseSectionDto?> UpdateSectionAsync(int sectionId, CourseSectionDto updated);
    Task<bool> DeleteSectionAsync(int sectionId);
    
    Task<IEnumerable<CourseDto>> GetAllCoursesAsync(string? category);

    Task<CourseDto?> GetCourseByIdAsync(int courseId);



}