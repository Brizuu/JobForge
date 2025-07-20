using JobForge.Data;
using JobForge.DbModels;
using JobForge.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace JobForge.Services
{
    public class InternshipService : IInternshipService
    {
        private readonly AppDbContext _context;

        public InternshipService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> CreateInternshipAsync(InternshipDto dto, Guid authorId)
    {
        var internship = new Internship
        {
            Id = Guid.NewGuid(),
            AuthorId = authorId,
            Title = dto.Title,
            Description = dto.Description,
            CompanyName = dto.CompanyName,
            Locations = dto.Locations.Select(l => new InternshipLocation
            {
                Id = Guid.NewGuid(),
                City = l.City,
                Country = l.Country
            }).ToList(),
            IsArchived = dto.IsArchived,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Duration = dto.Duration,
            EmploymentType = dto.EmploymentType,
            Stipend = dto.Stipend,
            Requirements = dto.Requirements,
            Benefits = dto.Benefits,
            ContactEmail = dto.ContactEmail
        };

        _context.Internships.Add(internship);
        await _context.SaveChangesAsync();
        return internship.Id;
    }

    public async Task<bool> UpdateInternshipAsync(Guid id, InternshipDto dto, Guid authorId)
    {
        var internship = await _context.Internships.Include(i => i.Locations).FirstOrDefaultAsync(i => i.Id == id && i.AuthorId == authorId);
        if (internship == null || !internship.IsArchived)
            return false;

        internship.Title = dto.Title;
        internship.Description = dto.Description;
        internship.CompanyName = dto.CompanyName;

        // Usuń stare lokalizacje i dodaj nowe
        _context.InternshipLocations.RemoveRange(internship.Locations);
        internship.Locations = dto.Locations.Select(l => new InternshipLocation
        {
            Id = Guid.NewGuid(),
            City = l.City,
            Country = l.Country
        }).ToList();

        internship.IsArchived = dto.IsArchived;
        internship.StartDate = dto.StartDate;
        internship.EndDate = dto.EndDate;
        internship.Duration = dto.Duration;
        internship.EmploymentType = dto.EmploymentType;
        internship.Stipend = dto.Stipend;
        internship.Requirements = dto.Requirements;
        internship.Benefits = dto.Benefits;
        internship.ContactEmail = dto.ContactEmail;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ToggleArchiveStatusAsync(Guid id, Guid authorId)
    {
        var internship = await _context.Internships.FirstOrDefaultAsync(i => i.Id == id && i.AuthorId == authorId);
        if (internship == null)
            return false;

        internship.IsArchived = !internship.IsArchived;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Internship>> GetInternshipsByAuthorAsync(Guid authorId)
    {
        return await _context.Internships
            .Include(i => i.Locations)
            .Where(i => i.AuthorId == authorId)
            .ToListAsync();
    }

    public async Task<List<InternshipApplication>> GetApplicationsForInternshipAsync(Guid internshipId, Guid authorId)
    {
        var internship = await _context.Internships.FirstOrDefaultAsync(i => i.Id == internshipId && i.AuthorId == authorId);
        if (internship == null)
            return null;

        return await _context.InternshipApplications.Where(a => a.InternshipId == internshipId).ToListAsync();
    }

    public async Task<bool> ReviewInternshipApplicationAsync(Guid applicationId, string status, Guid reviewerId)
    {
        // Pobierz aplikację
        var application = await _context.InternshipApplications
            .FirstOrDefaultAsync(a => a.Id == applicationId);

        if (application == null)
            return false;

        // Pobierz staż powiązany z aplikacją
        var internship = await _context.Internships
            .FirstOrDefaultAsync(i => i.Id == application.InternshipId);

        if (internship == null || internship.AuthorId != reviewerId)
            return false;

        // Zakładam, że w InternshipApplication jest pole Status typu string
        application.Status = status;
        await _context.SaveChangesAsync();

        return true;
    }


    public async Task<bool> ApplyForInternshipAsync(InternshipApplicationDto dto, Guid userId)
    {
        var internship = await _context.Internships.FirstOrDefaultAsync(i => i.Id == dto.InternshipId && !i.IsArchived);
        if (internship == null)
            return false;

        var application = new InternshipApplication
        {
            InternshipId = dto.InternshipId,
            UserId = userId,
            ApplicantName = dto.ApplicantName,
            ContactEmail = dto.ContactEmail,
            ContactPhone = dto.ContactPhone,
            CoverLetter = dto.CoverLetter,
            AppliedAt = DateTime.UtcNow,
            Status = "Pending" // jeśli masz status w modelu
        };

        _context.InternshipApplications.Add(application);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<InternshipApplication>> GetUserInternshipApplicationsAsync(Guid userId)
    {
        return await _context.InternshipApplications
            .Where(a => a.UserId == userId)
            .ToListAsync();
    }

    public async Task<List<Internship>> GetAllAvailableInternshipsAsync()
    {
        return await _context.Internships
            .Include(i => i.Locations)
            .Where(i => !i.IsArchived)
            .ToListAsync();
    }

    public async Task<Internship> GetInternshipDetailsAsync(Guid id)
    {
        return await _context.Internships
            .Include(i => i.Locations)
            .FirstOrDefaultAsync(i => i.Id == id && !i.IsArchived);
    }
    }
}
