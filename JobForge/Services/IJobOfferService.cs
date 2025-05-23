using JobForge.DbModels;
using JobForge.Models;

namespace JobForge.Services;

public interface IJobOfferService
{
    Task<int> CreateJobOfferAsync(JobOfferDto dto);
    Task<JobOfferDto?> GetJobOfferByIdAsync(int id);
    Task<bool> ArchiveJobOfferAsync(int jobOfferId, bool isArchived);

    Task<bool> DeleteJobOfferAsync(int jobOfferId);

    Task<int> CreateJobApplicationAsync(JobApplicationsDto dto, Guid userId);

}