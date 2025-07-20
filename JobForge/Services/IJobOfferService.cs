using JobForge.DbModels;
using JobForge.Models;

namespace JobForge.Services;

public interface IJobOfferService
{
    Task<bool> AddJobOfferAsync(JobOfferDto dto, Guid userId);
    Task<List<object>> GetAllJobOffersAsync();
    
    Task<JobOfferDto?> GetJobOfferByIdAsync(int id);

    Task<(bool Success, string Message)> ApplyToJobOfferAsync(int jobOfferId, Guid userId);
    
    Task<List<JobApplicationsDto>> GetUserJobApplicationsAsync(Guid userId);
    
    Task<bool> ArchiveJobOfferAsync(int jobOfferId, bool isArchived);

    Task<List<JobApplication>> GetApplicationsWithCVsForOfferAsync(int jobOfferId, Guid employerId);
    
    Task<bool> ReviewApplicationAsync(int applicationId, string newStatus, Guid reviewerUserId);


    
    // Task<bool> ArchiveJobOfferAsync(int jobOfferId, bool isArchived);
    //
    // Task<bool> DeleteJobOfferAsync(int jobOfferId);
    //
    // // Task<JobApplication> ApplyToJobOfferAsync(ApplyToJobOfferDto dto, Guid userId);
    
    Task AddFavoriteAsync(int jobOfferId, Guid userId);
    Task<List<FavoriteJobOfferDetailDto>> GetFavoritesByUserAsync(Guid userId);

}