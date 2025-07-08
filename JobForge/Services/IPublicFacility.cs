using System.Security.Claims;
using JobForge.DbModels;
using JobForge.Models;

namespace JobForge.Services;

public interface IPublicFacility
{
    Task<(bool Success, IEnumerable<string> Errors)> RegisterWithSupervisorAsync(RegisterDto dto, ClaimsPrincipal supervisorPrincipal);
    Task<object> GetUserCvForSupervisorAsync(Guid userId, Guid supervisorId);
    Task<IEnumerable<JobApplication>> GetUserApplicationsForSupervisorAsync(Guid userId, Guid supervisorId);
    
    Task<IEnumerable<UserCourse>> GetUserCoursesForSupervisorAsync(Guid userId, Guid supervisorId);
    
    Task<IEnumerable<WorkExperience>> GetUserWorkExperiencesForSupervisorAsync(Guid userId, Guid supervisorId);
}