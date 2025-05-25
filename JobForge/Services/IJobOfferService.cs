using JobForge.DbModels;
using JobForge.Models;

namespace JobForge.Services;

public interface IJobOfferService
{
    Task<int> CreateJobOfferAsync(JobOfferDto dto);
    Task<JobOfferDto?> GetJobOfferByIdAsync(int id);
    Task<bool> ArchiveJobOfferAsync(int jobOfferId, bool isArchived);

    Task<bool> DeleteJobOfferAsync(int jobOfferId);

    Task<JobApplication> ApplyToJobOfferAsync(ApplyToJobOfferDto dto, Guid userId);
    
    Task AddFavoriteAsync(int jobOfferId, Guid userId);
    Task<List<FavoriteJobOfferDetailDto>> GetFavoritesByUserAsync(Guid userId);

}