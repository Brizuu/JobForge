using JobForge.DbModels;
using JobForge.Models;

namespace JobForge.Services;

public interface ICourseService
{
    Task<Course> CreateCourseAsync(Guid userId, CourseDto dto);
    
    Task<Course?> UpdateCourseAsync(int courseId, CourseDto updatedDto);

    Task<bool> DeleteCourseAsync(int courseId);
    
    Task<List<Course>> GetCoursesByCreatorAsync(Guid creatorId);
    
    Task<CourseSection> AddSectionAsync(int courseId, CourseSectionDto sectionDto);
    Task<CourseSection?> UpdateSectionAsync(int sectionId, CourseSectionDto updatedDto);


    Task<bool> DeleteSectionAsync(int sectionId);
    
    Task<IEnumerable<Course>> GetAllCoursesAsync(string? category);

    Task<Course?> GetCourseByIdAsync(int courseId);

    Task AddUserCourseAsync(Guid userId, UserCourseCreateDto dto);
    Task UpdateCompletionPercentageAsync(Guid userId, UserCourseUpdateDto dto);
    Task<IEnumerable<UserCourseDto>> GetCoursesByUserIdAsync(Guid userId);
    Task AddCompletedCourseTitleAsync(Guid userId, int courseId);



}