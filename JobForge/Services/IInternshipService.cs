using JobForge.DbModels;
using JobForge.Models;

namespace JobForge.Services;

public interface IInternshipService
{
    Task<IntershipDto> CreateInternshipAsync(IntershipDto internshipDto, Guid authorId);
    Task<bool> DeleteInternshipAsync(Guid internshipId);

    Task<IntershipDto?> GetInternshipByIdAsync(Guid id);
    Task<List<IntershipDto>> GetAllInternshipsAsync();
    // Task<IEnumerable<InternshipApplicationDto>> GetAllApplicationsAsync();
    Task<IEnumerable<InternshipApplication>> GetApplicationsAsync(Guid? internshipId = null);

    
    Task<InternshipApplicationDto> CreateApplicationAsync(InternshipApplicationDto applicationDto, Guid userId);
    Task<bool> DeleteApplicationAsync(Guid applicationId);
   
    

}