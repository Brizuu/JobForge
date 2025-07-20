using System.Security.Claims;
using JobForge.DbModels;
using JobForge.Models;

namespace JobForge.Services;

public interface IPublicFacility
{
    Task<bool> RegisterUserAsync(ApplicationUser user, string password, Guid creatorId);
    Task<List<object>> GetUsersByCompanyIdAsync(Guid creatorId);
    Task<bool> AssignSupervisorAsync(Guid userId, Guid supervisorId, Guid executorId);
    Task<object?> GetUserDetailsAsync(Guid userId, Guid executorId);
    Task<object> GetStatisticsAsync(Guid? userId, Guid executorId);
}