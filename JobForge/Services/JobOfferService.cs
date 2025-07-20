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

    public async Task<bool> AddJobOfferAsync(JobOfferDto dto, Guid userId)
    {
        if (dto == null) return false;

        var jobOffer = new JobOffer
        {
            UserId = userId,
            IsArchived = dto.IsArchived,
            JobTitle = dto.JobTitle,
            Description = dto.Description,
            EmploymentType = dto.EmploymentType,
            SalaryFrom = dto.SalaryFrom,
            SalaryTo = dto.SalaryTo,
            PostedDate = dto.PostedDate,
            ExpirationDate = dto.ExpirationDate,
            CompanyName = dto.CompanyName,
            Category = dto.Category,
            ExperienceLevel = dto.ExperienceLevel,
            ApplyLink = dto.ApplyLink,
            PostViews = dto.PostViews,
            Applicants = dto.Applicants,
            ActiveWorkers = dto.ActiveWorkers,
            Locations = dto.Locations.Select(loc => new JobOfferLocation
            {
                LocationType = loc.LocationType,
                City = loc.City,
                Country = loc.Country
            }).ToList(),
            Technologies = dto.Technologies.Select(tech => new JobOfferTechnology
            {
                Name = tech.Name,
                ExperienceLevel = tech.ExperienceLevel,
                Description = tech.Description
            }).ToList()
        };

        _context.JobOffers.Add(jobOffer);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }
    
    public async Task<List<object>> GetAllJobOffersAsync()
    {
        return await _context.JobOffers
            .Include(j => j.Locations)
            .Include(j => j.Technologies)
            .Where(j => !j.IsArchived)  // Tylko nie zarchiwizowane oferty
            .Select(j => new
            {
                JobOfferId = j.Id,
                JobOffer = new JobOfferDto
                {
                    JobTitle = j.JobTitle,
                    Description = j.Description,
                    EmploymentType = j.EmploymentType,
                    SalaryFrom = j.SalaryFrom,
                    SalaryTo = j.SalaryTo,
                    ExpirationDate = j.ExpirationDate,
                    CompanyName = j.CompanyName,
                    ExperienceLevel = j.ExperienceLevel,
                    Locations = j.Locations.Select(l => new JobOfferLocationDto
                    {
                        Id = l.Id,
                        JobOfferId = l.JobOfferId,
                        LocationType = l.LocationType,
                        City = l.City,
                        Country = l.Country
                    }).ToList(),
                    Technologies = j.Technologies.Select(t => new JobOfferTechnologyDto
                    {
                        Id = t.Id,
                        JobOfferId = t.JobOfferId,
                        Name = t.Name,
                        ExperienceLevel = t.ExperienceLevel,
                        Description = t.Description
                    }).ToList()
                }
            })
            .ToListAsync<object>();
    }

    
    public async Task<JobOfferDto?> GetJobOfferByIdAsync(int id)
    {
        var offer = await _context.JobOffers
            .Include(o => o.Locations)
            .Include(o => o.Technologies)
            .FirstOrDefaultAsync(o => o.Id == id && !o.IsArchived);

        if (offer == null)
            return null;

        return new JobOfferDto
        {
            IsArchived = offer.IsArchived,
            JobTitle = offer.JobTitle,
            Description = offer.Description,
            EmploymentType = offer.EmploymentType,
            SalaryFrom = offer.SalaryFrom,
            SalaryTo = offer.SalaryTo,
            PostedDate = offer.PostedDate,
            ExpirationDate = offer.ExpirationDate,
            CompanyName = offer.CompanyName,
            Category = offer.Category,
            ExperienceLevel = offer.ExperienceLevel,
            ApplyLink = offer.ApplyLink,
            PostViews = offer.PostViews,
            Applicants = offer.Applicants,
            ActiveWorkers = offer.ActiveWorkers,
            Locations = offer.Locations.Select(l => new JobOfferLocationDto
            {
                Id = l.Id,
                JobOfferId = l.JobOfferId,
                LocationType = l.LocationType,
                City = l.City,
                Country = l.Country
            }).ToList(),
            Technologies = offer.Technologies.Select(t => new JobOfferTechnologyDto
            {
                Id = t.Id,
                JobOfferId = t.JobOfferId,
                Name = t.Name,
                ExperienceLevel = t.ExperienceLevel,
                Description = t.Description
            }).ToList()
        };
    }
    
    public async Task<(bool, string)> ApplyToJobOfferAsync(int jobOfferId, Guid userId)
    {
        var jobOffer = await _context.JobOffers
            .FirstOrDefaultAsync(j => j.Id == jobOfferId && !j.IsArchived);

        if (jobOffer == null)
            return (false, "Job offer not found or archived.");
        
        var userCv = await _context.GeneratedCVs
            .Where(cv => cv.UserId == userId)
            .FirstOrDefaultAsync();

        if (userCv == null)
            return (false, "User CV not found.");
        
        var existingApplication = await _context.JobApplications
            .Where(app => app.JobOfferId == jobOfferId && app.UserId == userId)
            .Where(app => app.Status != "Rejected") // tylko te, które nie są odrzucone
            .FirstOrDefaultAsync();

        if (existingApplication != null)
            return (false, "You have already applied to this job offer and your application is not rejected.");
        
        var application = new JobApplication
        {
            JobOfferId = jobOfferId,
            CvId = userCv.Id,
            UserId = userId,
            AppliedAt = DateTime.UtcNow,
            Status = "Pending"
        };

        _context.JobApplications.Add(application);
        await _context.SaveChangesAsync();

        return (true, "Application submitted successfully.");
    }



    
    // public async Task<bool> ArchiveJobOfferAsync(int jobOfferId, bool isArchived)
    // {
    //     var jobOffer = await _context.JobOffers.FindAsync(jobOfferId);
    //     if (jobOffer == null)
    //         return false;
    //
    //     jobOffer.IsArchived = isArchived;
    //     await _context.SaveChangesAsync();
    //     return true;
    // }

    
    // public async Task<bool> DeleteJobOfferAsync(int jobOfferId)
    // {
    //     var jobOffer = await _context.JobOffers
    //         .Include(j => j.Technologies)
    //         .FirstOrDefaultAsync(j => j.Id == jobOfferId);
    //
    //     if (jobOffer == null)
    //         return false;
    //
    //     // Usuń technologie powiązane (jeśli brak kaskady)
    //     _context.JobOfferTechnologies.RemoveRange(jobOffer.Technologies);
    //
    //     // Usuń ogłoszenie
    //     _context.JobOffers.Remove(jobOffer);
    //
    //     await _context.SaveChangesAsync();
    //     return true;
    // }

    
    // public async Task<JobApplication> ApplyToJobOfferAsync(ApplyToJobOfferDto dto, Guid userId)
    // {
    //     var offer = await _context.JobOffers.FindAsync(dto.JobOfferId);
    //     if (offer == null)
    //         throw new Exception("Job offer not found.");
    //
    //     var cv = await _context.GeneratedCVs.FindAsync(dto.CvId);
    //     if (cv == null || cv.UserId != userId)
    //         throw new Exception("Invalid CV.");
    //
    //     var deserializedCv = JsonSerializer.Deserialize<object>(cv.ContentJson);
    //
    //     var application = new JobApplication
    //     {
    //         JobOfferId = dto.JobOfferId,
    //         CvId = dto.CvId,
    //         UserId = userId,
    //         AppliedAt = DateTime.UtcNow,
    //         Status = "Pending",
    //         JobOffer = offer,
    //         DeserializedCv = deserializedCv
    //     };
    //
    //     _context.JobApplications.Add(application);
    //     await _context.SaveChangesAsync();
    //
    //     return application;
    // }

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
    
    public async Task<List<JobApplicationsDto>> GetUserJobApplicationsAsync(Guid userId)
    {
        return await _context.JobApplications
            .Where(app => app.UserId == userId)
            .Include(app => app.JobOffer)
            .OrderByDescending(app => app.AppliedAt)
            .Select(app => new JobApplicationsDto
            {
                Id = app.Id,
                JobOfferId = app.JobOfferId,
                JobTitle = app.JobOffer.JobTitle,
                CompanyName = app.JobOffer.CompanyName,
                AppliedAt = app.AppliedAt,
                Status = app.Status
            })
            .ToListAsync();
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
    
    public async Task<bool> ReviewApplicationAsync(int applicationId, string newStatus, Guid reviewerUserId)
    {
        var application = await _context.JobApplications
            .FirstOrDefaultAsync(a => a.Id == applicationId);

        if (application == null)
            return false;

        var jobOffer = await _context.JobOffers
            .FirstOrDefaultAsync(o => o.Id == application.JobOfferId);

        if (jobOffer == null || jobOffer.UserId != reviewerUserId)
            return false;

        application.Status = newStatus;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<JobApplication>> GetApplicationsWithCVsForOfferAsync(int jobOfferId, Guid employerId)
    {
        var offer = await _context.JobOffers
            .AsNoTracking()
            .FirstOrDefaultAsync(j => j.Id == jobOfferId && j.UserId == employerId);

        if (offer == null)
            return new List<JobApplication>();

        var applications = await _context.JobApplications
            .Where(a => a.JobOfferId == jobOfferId)
            .Include(a => a.CV)
            .ThenInclude(cv => cv.Educations)
            .Include(a => a.CV)
            .ThenInclude(cv => cv.WorkExperiences)
            .Include(a => a.CV)
            .ThenInclude(cv => cv.Languages)
            .Include(a => a.CV)
            .ThenInclude(cv => cv.SoftSkills)
            .Include(a => a.CV)
            .ThenInclude(cv => cv.TechnicalSkills)
            .Include(a => a.CV)
            .ThenInclude(cv => cv.Interests)
            .Include(a => a.CV)
            .ThenInclude(cv => cv.UserCourse)
            .ToListAsync();

        return applications;
    }







}
