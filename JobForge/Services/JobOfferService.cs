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

    
    public async Task<int> CreateJobApplicationAsync(JobApplicationsDto dto, Guid userId)
    {
        var personalInfo = new PersonalInformation
        {
            UserId = userId,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            PhoneNumber = dto.PhoneNumber,
            EmailAddress = dto.EmailAddress,
            LinkedinUrl = dto.LinkedinUrl,
            Summary = dto.Summary,
            TechnicalSkills = dto.TechnicalSkills ?? new List<string>(),
            SoftSkills = dto.SoftSkills ?? new List<string>(),
            Interests = dto.Interests ?? new List<string>(),
            Certificates = dto.Certificates ?? new List<string>(),
            Courses = dto.Courses ?? new List<string>(),
            WorkExperiences = dto.WorkExperiences?.Select(we => new WorkExperience
            {
                UserId = userId,
                CompanyName = we.CompanyName,
                EmploymentDateStart = we.EmploymentDateStart,
                EmploymentDateEnd = we.EmploymentDateEnd,
                Responsibilities = we.Responsibilities
            }).ToList() ?? new List<WorkExperience>(),
            Educations = dto.Educations?.Select(ed => new Education
            {
                UserId = userId,
                SchoolName = ed.SchoolName,
                Major = ed.Major,
                EducationDateStart = ed.EducationDateStart,
                EducationDateEnd = ed.EducationDateEnd
            }).ToList() ?? new List<Education>(),
            Languages = dto.Languages?.Select(lang => new Language
            {
                UserId = userId,
                LanguageName = lang.LanguageName,
                ProficiencyLevel = lang.ProficiencyLevel
            }).ToList() ?? new List<Language>()
        };

        var jobApplication = new JobApplication
        {
            JobOfferId = dto.JobOfferId,
            UserId = userId,
            PersonalInformation = personalInfo,
            Status = "Pending",
            ApplicationDate = DateTime.UtcNow
        };

        _context.JobApplications.Add(jobApplication);
        await _context.SaveChangesAsync();

        return jobApplication.Id;
    }


}
