using JobForge.Data;
using JobForge.DbModels;
using JobForge.Models;
using Microsoft.EntityFrameworkCore;

namespace JobForge.Services;

public class GrantService : IGrantService
{
    private readonly AppDbContext _context;

    public GrantService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> CreateGrantAsync(GrantDto dto, Guid authorId)
    {
        var grant = new Grant
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Description = dto.Description,
            Amount = dto.Amount,
            ApplicationDeadline = dto.ApplicationDeadline ?? dto.ApplicationEndDate,
            ApplicationStartDate = dto.ApplicationStartDate,
            ApplicationEndDate = dto.ApplicationEndDate,
            IsArchived = dto.IsArchived,
            FundingOrganization = dto.FundingOrganization,
            EligibilityCriteria = dto.EligibilityCriteria ?? string.Empty,
            ApplicationProcess = dto.ApplicationProcess ?? string.Empty,
            ContactEmail = dto.ContactEmail ?? string.Empty,
            Region = dto.Region ?? string.Empty,
            AuthorId = authorId
        };

        _context.Grants.Add(grant);
        await _context.SaveChangesAsync();
        return grant.Id;
    }

    public async Task<bool> UpdateGrantAsync(Guid id, GrantDto dto, Guid authorId)
    {
        var grant = await _context.Grants.FirstOrDefaultAsync(g => g.Id == id && g.AuthorId == authorId && g.IsArchived);

        if (grant == null) return false;

        grant.Title = dto.Title;
        grant.Description = dto.Description;
        grant.Amount = dto.Amount;
        grant.ApplicationDeadline = dto.ApplicationDeadline ?? dto.ApplicationEndDate;
        grant.ApplicationStartDate = dto.ApplicationStartDate;
        grant.ApplicationEndDate = dto.ApplicationEndDate;
        grant.FundingOrganization = dto.FundingOrganization;
        grant.EligibilityCriteria = dto.EligibilityCriteria ?? string.Empty;
        grant.ApplicationProcess = dto.ApplicationProcess ?? string.Empty;
        grant.ContactEmail = dto.ContactEmail ?? string.Empty;
        grant.Region = dto.Region ?? string.Empty;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ToggleArchiveStatusAsync(Guid grantId, Guid authorId)
    {
        var grant = await _context.Grants.FirstOrDefaultAsync(g => g.Id == grantId && g.AuthorId == authorId);

        if (grant == null) return false;

        grant.IsArchived = !grant.IsArchived;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Grant>> GetAllAvailableGrantsAsync()
    {
        return await _context.Grants
            .Where(g => !g.IsArchived && DateTime.UtcNow >= g.ApplicationStartDate && DateTime.UtcNow <= g.ApplicationEndDate)
            .ToListAsync();
    }

    public async Task<Grant?> GetGrantDetailsAsync(Guid grantId)
    {
        return await _context.Grants.FirstOrDefaultAsync(g => g.Id == grantId && !g.IsArchived);
    }

    public async Task<bool> ApplyForGrantAsync(GrantApplicationDto dto, Guid userId)
    {
        if (!await _context.Grants.AnyAsync(g => g.Id == dto.GrantId && !g.IsArchived)) return false;

        var app = new GrantApplication
        {
            Id = Guid.NewGuid(),
            GrantId = dto.GrantId,
            UserId = userId,
            AppliedAt = DateTime.UtcNow,
            Requirements = dto.Requirements,
            Justification = dto.Justification,
            RequestedAmount = dto.RequestedAmount,
            ApplicantName = dto.ApplicantName,
            ContactEmail = dto.ContactEmail,
            ContactPhone = dto.ContactPhone,
            Region = dto.Region
        };

        _context.GrantApplications.Add(app);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<GrantApplication>> GetUserGrantApplicationsAsync(Guid userId)
    {
        return await _context.GrantApplications
            .Where(a => a.UserId == userId)
            .ToListAsync();
    }

    public async Task<List<GrantApplication>> GetGrantApplicationsForGrantAsync(Guid grantId, Guid employerId)
    {
        var grant = await _context.Grants.FirstOrDefaultAsync(g => g.Id == grantId && g.AuthorId == employerId);
        if (grant == null) return new List<GrantApplication>();

        return await _context.GrantApplications.Where(a => a.GrantId == grantId).ToListAsync();
    }

    public async Task<bool> ReviewGrantApplicationAsync(Guid applicationId, string status, Guid reviewerId)
    {
        var application = await _context.GrantApplications.FindAsync(applicationId);
        if (application == null)
            return false;

        var grant = await _context.Grants.FindAsync(application.GrantId);
        if (grant == null || grant.AuthorId != reviewerId)
            return false;

        application.Status = status;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Grant>> GetMyCreatedGrantsAsync(Guid authorId)
    {
        return await _context.Grants
            .Where(g => g.AuthorId == authorId)
            .ToListAsync();
    }


}