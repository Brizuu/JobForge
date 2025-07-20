using JobForge.Models;
using JobForge.DbModels;

public interface IGrantService
{
    Task<Guid> CreateGrantAsync(GrantDto dto, Guid authorId);
    Task<bool> UpdateGrantAsync(Guid id, GrantDto dto, Guid authorId);
    Task<bool> ToggleArchiveStatusAsync(Guid grantId, Guid authorId);
    Task<List<Grant>> GetAllAvailableGrantsAsync();
    Task<Grant?> GetGrantDetailsAsync(Guid grantId);

    Task<bool> ApplyForGrantAsync(GrantApplicationDto dto, Guid userId);
    Task<List<GrantApplication>> GetUserGrantApplicationsAsync(Guid userId);

    Task<List<GrantApplication>> GetGrantApplicationsForGrantAsync(Guid grantId, Guid employerId);
    Task<bool> ReviewGrantApplicationAsync(Guid applicationId, string newStatus, Guid reviewerId);

    Task<List<Grant>> GetMyCreatedGrantsAsync(Guid authorId);

}