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

    public async Task<GrantDto> CreateGrantAsync(GrantDto dto, Guid authorId)
    {
        var grant = new Grant
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Description = dto.Description,
            ApplicationStartDate = dto.ApplicationStartDate,
            ApplicationEndDate = dto.ApplicationEndDate,
            Amount = dto.Amount,
            AuthorId = authorId
        };

        _context.Grants.Add(grant);
        await _context.SaveChangesAsync();

        return dto;
    }

    public async Task<GrantDto?> GetGrantByIdAsync(Guid id)
    {
        var g = await _context.Grants.FindAsync(id);
        if (g == null) return null;

        return new GrantDto
        {
            Title = g.Title,
            Description = g.Description,
            ApplicationStartDate = g.ApplicationStartDate,
            ApplicationEndDate = g.ApplicationEndDate,
            Amount = g.Amount
        };
    }

    public async Task<List<GrantDto>> GetAllGrantsAsync()
    {
        return await _context.Grants.Select(g => new GrantDto
        {
            Title = g.Title,
            Description = g.Description,
            ApplicationStartDate = g.ApplicationStartDate,
            ApplicationEndDate = g.ApplicationEndDate,
            Amount = g.Amount
        }).ToListAsync();
    }

    public async Task<GrantApplication> CreateApplicationAsync(GrantApplicationDto dto, Guid userId)
    {
        var exists = await _context.Grants.AnyAsync(g => g.Id == dto.GrantId);
        if (!exists)
            throw new KeyNotFoundException($"Grant with ID '{dto.GrantId}' not found.");

        var app = new GrantApplication
        {
            Id = Guid.NewGuid(),
            GrantId = dto.GrantId,
            UserId = userId,
            AppliedAt = DateTime.UtcNow,
            Requirements = dto.Requirements
        };

        _context.GrantApplications.Add(app);
        await _context.SaveChangesAsync();

        return app;
    }

    public async Task<IEnumerable<GrantApplication>> GetApplicationsAsync(Guid? grantId = null)
    {
        var query = _context.GrantApplications.AsQueryable();
        if (grantId.HasValue)
            query = query.Where(a => a.GrantId == grantId.Value);

        return await query.ToListAsync();
    }

    public async Task<List<GrantApplication>> GetApplicationsByGrantIdAsync(Guid grantId)
    {
        return await _context.GrantApplications
            .Where(a => a.GrantId == grantId)
            .ToListAsync();
    }

    public async Task<bool> DeleteGrantAsync(Guid id)
    {
        var grant = await _context.Grants.FindAsync(id);
        if (grant == null) return false;

        _context.Grants.Remove(grant);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteApplicationAsync(Guid id)
    {
        var app = await _context.GrantApplications.FindAsync(id);
        if (app == null) return false;

        _context.GrantApplications.Remove(app);
        await _context.SaveChangesAsync();
        return true;
    }
}