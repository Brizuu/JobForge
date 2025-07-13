using JobForge.DbModels;
using JobForge.Models;

namespace JobForge.Services;

public interface ICourseService
{
    // Kursy tworzone przez użytkowników (nauczyciele/admini)
    Task<Course> CreateCourseAsync(Guid userId, CourseDto dto);
    Task<Course?> UpdateCourseAsync(Guid courseId, CourseDto updatedDto);
    Task<bool> DeleteCourseAsync(Guid courseId);
    Task<List<Course>> GetCoursesByCreatorAsync(Guid creatorId);
    Task<IEnumerable<object>> GetAllCoursesAsync(string? category);

    Task<bool> ChangeCoursePublishedStateAsync(Guid courseId, Guid userId, bool newState);

    Task<Course?> GetCourseByIdAsync(Guid courseId);

    
    Task<CourseSection?> GetNextSectionAsync(Guid userId, Guid courseId);

    
    // Sekcje kursu
    Task<CourseSection> AddSectionAsync(Guid courseId, CourseSectionDto sectionDto);
    Task<CourseSection?> UpdateSectionAsync(int sectionId, CourseSectionEditDto updatedDto);
    Task<bool> DeleteSectionAsync(int sectionId);

    // Kursy użytkownika (powiązane z CV)
    Task<bool> StartCourseAsync(Guid userId, Guid courseId);

    Task UpdateCompletionPercentageAsync(Guid userId, Guid courseId, double newPercentage);
    Task<IEnumerable<UserCourseDto>> GetCoursesByUserIdAsync(Guid userId);
}