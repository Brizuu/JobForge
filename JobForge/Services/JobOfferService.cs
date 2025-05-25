using System.Text.Json;
using JobForge.Data;
using JobForge.DbModels;
using JobForge.Models;
using Microsoft.EntityFrameworkCore;

namespace JobForge.Services;

public class JobOfferService : IJobOfferService
{
    private readonly AppDbContext _context;

    public JobOfferService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> CreateJobOfferAsync(JobOfferDto dto)
    {
        var jobOffer = new JobOffer
        {
            UserId = dto.UserId,
            JobTitle = dto.JobTitle,
            Description = dto.Description,
            Location = dto.Location,
            EmploymentType = dto.EmploymentType,
            SalaryFrom = dto.SalaryFrom,
            SalaryTo = dto.SalaryTo,
            PostedDate = dto.PostedDate.ToUniversalTime(),
            ExpirationDate = dto.ExpirationDate.ToUniversalTime(),
            CompanyName = dto.CompanyName,
            Category = dto.Category,
            ExperienceLevel = dto.ExperienceLevel,
            ApplyLink = dto.ApplyLink,
            PostViews = dto.PostViews,
            Applicants = dto.Applicants,
            ActiveWorkers = dto.ActiveWorkers,
            Technologies = dto.Technologies?
                .Select(t => new JobOfferTechnology { Name = t.Name })
                .ToList() ?? new List<JobOfferTechnology>()


        };

        _context.JobOffers.Add(jobOffer);
        await _context.SaveChangesAsync();

        return jobOffer.Id;
    }
    
    public async Task<JobOfferDto?> GetJobOfferByIdAsync(int id)
    {
        var jobOffer = await _context.JobOffers
            .Where(j => j.Id == id)
            .Select(j => new JobOfferDto
            {
                Id = j.Id,
                UserId = j.UserId,
                JobTitle = j.JobTitle,
                Description = j.Description,
                Location = j.Location,
                EmploymentType = j.EmploymentType,
                SalaryFrom = j.SalaryFrom,
                SalaryTo = j.SalaryTo,
                PostedDate = DateTime.SpecifyKind(j.PostedDate, DateTimeKind.Utc),
                ExpirationDate = DateTime.SpecifyKind(j.ExpirationDate, DateTimeKind.Utc),
                CompanyName = j.CompanyName,
                Category = j.Category,
                ExperienceLevel = j.ExperienceLevel,
                ApplyLink = j.ApplyLink,
                PostViews = j.PostViews,
                Applicants = j.Applicants,
                ActiveWorkers = j.ActiveWorkers,
                Technologies = j.Technologies.Select(t => new JobOfferTechnologyDto
                {
                    Name = t.Name
                }).ToList()
            })
            .FirstOrDefaultAsync();

        return jobOffer;
    }
    
    public async Task<bool> ArchiveJobOfferAsync(int jobOfferId, bool isArchived)
    {
        var jobOffer = await _context.JobOffers.FindAsync(jobOfferId);
        if (jobOffer == null)
            return false;

        jobOffer.IsArchived = isArchived;
        await _context.SaveChangesAsync();
        return true;
    }

    
    public async Task<bool> DeleteJobOfferAsync(int jobOfferId)
    {
        var jobOffer = await _context.JobOffers
            .Include(j => j.Technologies)
            .FirstOrDefaultAsync(j => j.Id == jobOfferId);

        if (jobOffer == null)
            return false;

        // Usuń technologie powiązane (jeśli brak kaskady)
        _context.JobOfferTechnologies.RemoveRange(jobOffer.Technologies);

        // Usuń ogłoszenie
        _context.JobOffers.Remove(jobOffer);

        await _context.SaveChangesAsync();
        return true;
    }

    
    public async Task<JobApplication> ApplyToJobOfferAsync(ApplyToJobOfferDto dto, Guid userId)
    {
        var offer = await _context.JobOffers.FindAsync(dto.JobOfferId);
        if (offer == null)
            throw new Exception("Job offer not found.");

        var cv = await _context.GeneratedCVs.FindAsync(dto.CvId);
        if (cv == null || cv.UserId != userId)
            throw new Exception("Invalid CV.");

        var deserializedCv = JsonSerializer.Deserialize<object>(cv.ContentJson);

        var application = new JobApplication
        {
            JobOfferId = dto.JobOfferId,
            CvId = dto.CvId,
            UserId = userId,
            AppliedAt = DateTime.UtcNow,
            Status = "Pending",
            JobOffer = offer,
            DeserializedCv = deserializedCv
        };

        _context.JobApplications.Add(application);
        await _context.SaveChangesAsync();

        return application;
    }

    public async Task AddFavoriteAsync(int jobOfferId, Guid userId)
    {
        var exists = await _context.FavoriteJobOffers
            .AnyAsync(f => f.UserId == userId && f.JobOfferId == jobOfferId);

        if (exists) return;

        var favorite = new FavoriteJobOffer
        {
            UserId = userId,
            JobOfferId = jobOfferId,
            AddedAt = DateTime.UtcNow
        };

        _context.FavoriteJobOffers.Add(favorite);
        await _context.SaveChangesAsync();
    }

    public async Task<List<FavoriteJobOfferDetailDto>> GetFavoritesByUserAsync(Guid userId)
    {
        return await _context.FavoriteJobOffers
            .Where(f => f.UserId == userId)
            .Join(_context.JobOffers,
                fav => fav.JobOfferId,
                job => job.Id,
                (fav, job) => new FavoriteJobOfferDetailDto
                {
                    JobOfferId = job.Id,
                    JobTitle = job.JobTitle,
                    CompanyName = job.CompanyName,
                    AddedAt = fav.AddedAt
                })
            .ToListAsync();
    }


}
