using JobForge.Models;
using JobForge.DbModels;

public interface IGrantService
{
    Task<GrantDto> CreateGrantAsync(GrantDto dto, Guid authorId);
    Task<GrantApplication> CreateApplicationAsync(GrantApplicationDto dto, Guid userId);
    Task<List<GrantDto>> GetAllGrantsAsync();
    Task<GrantDto?> GetGrantByIdAsync(Guid id);
    Task<IEnumerable<GrantApplication>> GetApplicationsAsync(Guid? grantId = null);
    Task<List<GrantApplication>> GetApplicationsByGrantIdAsync(Guid grantId);
    Task<bool> DeleteGrantAsync(Guid id);
    Task<bool> DeleteApplicationAsync(Guid id);
}