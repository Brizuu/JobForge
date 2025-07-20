using System.Security.Authentication;
using System.Security.Claims;
using JobForge.Data;
using JobForge.DbModels;
using JobForge.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JobForge.Services;

public class PublicFacility : IPublicFacility
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly AppDbContext _context;

    public PublicFacility(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, AppDbContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
    }

    public async Task<bool> RegisterUserAsync(ApplicationUser user, string password, Guid creatorId)
    {
        var creator = await _userManager.FindByIdAsync(creatorId.ToString());
        if (creator == null || creator.CompanyId == null) return false;

        user.CompanyId = creator.CompanyId;
        var result = await _userManager.CreateAsync(user, password);
        return result.Succeeded;
    }

    public async Task<List<object>> GetUsersByCompanyIdAsync(Guid creatorId)
    {
        var creator = await _userManager.FindByIdAsync(creatorId.ToString());
        if (creator == null || creator.CompanyId == null)
            return new List<object>();

        var users = await _userManager.Users
            .Where(u => u.CompanyId == creator.CompanyId)
            .Select(u => new
            {
                id = u.Id,
                firstName = u.FirstName,
                lastName = u.LastName,
                supervisorId = u.SupervisorId,
                companyId = u.CompanyId,
                userName = u.UserName,
                email = u.Email
            })
            .ToListAsync();

        return users.Cast<object>().ToList();
    }


    public async Task<bool> AssignSupervisorAsync(Guid userId, Guid supervisorId, Guid executorId)
    {
        var executor = await _userManager.FindByIdAsync(executorId.ToString());
        var user = await _userManager.FindByIdAsync(userId.ToString());
        var supervisor = await _userManager.FindByIdAsync(supervisorId.ToString());

        if (executor?.CompanyId != user?.CompanyId || user.CompanyId != supervisor?.CompanyId)
            return false;

        user.SupervisorId = supervisorId;
        await _userManager.UpdateAsync(user);
        return true;
    }

    public async Task<object?> GetUserDetailsAsync(Guid userId, Guid executorId)
    {
        var executor = await _userManager.FindByIdAsync(executorId.ToString());
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId.ToString());

        if (user == null || executor.CompanyId != user.CompanyId) return null;

        var cv = await _context.GeneratedCVs
            .Include(c => c.Educations)
            .Include(c => c.WorkExperiences)
            .Include(c => c.Languages)
            .Include(c => c.SoftSkills)
            .Include(c => c.TechnicalSkills)
            .Include(c => c.Interests)
            .Include(c => c.UserCourse)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        var jobApps = await _context.JobApplications
            .Where(j => j.UserId == userId)
            .ToListAsync();

        return new
        {
            UserId = user.Id,
            CV = cv,
            JobApplications = jobApps
        };
    }

    public async Task<object> GetStatisticsAsync(Guid? userId, Guid executorId)
    {
        var executor = await _userManager.FindByIdAsync(executorId.ToString());
        if (executor?.CompanyId == null)
        {
            Console.WriteLine("Unauthorized: executor or CompanyId is null");
            return new { Error = "Unauthorized" };
        }

        var users = _userManager.Users
            .Where(u => u.CompanyId == executor.CompanyId);

        if (userId.HasValue)
        {
            Console.WriteLine($"----Filtering for userId: {userId}");
            users = users.Where(u => u.Id == userId.Value.ToString());
        }

        // Pobieramy userIds jako Guid
        var userIdStrings = await users.Select(u => u.Id).ToListAsync();
        Console.WriteLine("User Ids strings:");
        foreach(var idStr in userIdStrings)
        {
            Console.WriteLine(idStr);
        }
        var userIds = userIdStrings.Select(Guid.Parse).ToList();

        Console.WriteLine($"-----UserIds for CompanyId {executor.CompanyId}: {string.Join(", ", userIds)}");

        var employedCount = await _context.EmploymentContracts
            .CountAsync(e => userIds.Contains(e.UserId) && e.Status == "Accepted");
        Console.WriteLine($"----Employed count: {employedCount}");

        var pendingJobs = await _context.JobApplications
            .CountAsync(j => userIds.Contains(j.UserId) && j.Status == "Pending");
        Console.WriteLine($"----Pending job applications count: {pendingJobs}");

        var completedCourses = await _context.UserCourses
            .CountAsync(c => userIds.Contains(c.UserId) && c.isCompleted);
        Console.WriteLine($"----Completed courses count: {completedCourses}");

        return new
        {
            Employed = employedCount,
            PendingJobApplications = pendingJobs,
            CompletedCourses = completedCourses
        };
    }




}