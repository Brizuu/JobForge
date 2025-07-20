using JobForge.DbModels;
using JobForge.Models;

namespace JobForge.Services;

public interface IInternshipService
{
    Task<Guid> CreateInternshipAsync(InternshipDto dto, Guid authorId);
    Task<bool> UpdateInternshipAsync(Guid id, InternshipDto dto, Guid authorId);
    Task<bool> ToggleArchiveStatusAsync(Guid id, Guid authorId);
    Task<List<Internship>> GetInternshipsByAuthorAsync(Guid authorId);
    Task<List<InternshipApplication>> GetApplicationsForInternshipAsync(Guid internshipId, Guid authorId);
    Task<bool> ReviewInternshipApplicationAsync(Guid applicationId, string status, Guid reviewerId);
    Task<bool> ApplyForInternshipAsync(InternshipApplicationDto dto, Guid userId);
    Task<List<InternshipApplication>> GetUserInternshipApplicationsAsync(Guid userId);
    Task<List<Internship>> GetAllAvailableInternshipsAsync();
    Task<Internship> GetInternshipDetailsAsync(Guid id);
   
    

}