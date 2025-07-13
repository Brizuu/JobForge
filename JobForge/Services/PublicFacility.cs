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

    public async Task<(bool Success, IEnumerable<string> Errors)> RegisterWithSupervisorAsync(RegisterDto dto, ClaimsPrincipal supervisorPrincipal)
    {
        var supervisor = await _userManager.GetUserAsync(supervisorPrincipal);

        if (supervisor == null || !await _userManager.IsInRoleAsync(supervisor, "PublicFacility"))
        {
            return (false, new[] { "Unauthorized: Only users with 'PublicFacility' role can register new users." });
        }

        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            // SupervisorId = Guid.Parse(supervisor.Id)
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            return (false, result.Errors.Select(e => e.Description));

        const string defaultRole = "free";
        if (!await _roleManager.RoleExistsAsync(defaultRole))
            await _roleManager.CreateAsync(new IdentityRole(defaultRole));

        await _userManager.AddToRoleAsync(user, defaultRole);

        return (true, null);
    }
    
    // public async Task<object> GetUserCvForSupervisorAsync(Guid userId, Guid supervisorId)
    // {
    //     var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId.ToString());
    //
    //     if (user == null)
    //         throw new KeyNotFoundException("Użytkownik nie istnieje.");
    //
    //     if (user.SupervisorId != supervisorId)
    //         throw new UnauthorizedAccessException("Nie masz dostępu do tego CV.");
    //
    //     var personalInfo = await _context.PersonalInformations
    //         .Include(p => p.WorkExperiences)
    //         .Include(p => p.Educations)
    //         .Include(p => p.Languages)
    //         .FirstOrDefaultAsync(p => p.UserId == userId);
    //
    //     if (personalInfo == null)
    //         throw new InvalidOperationException("Brak danych osobowych użytkownika.");
    //
    //     var result = new
    //     {
    //         PersonalInformation = personalInfo,
    //         WorkExperience = personalInfo.WorkExperiences,
    //         Education = personalInfo.Educations,
    //         Languages = personalInfo.Languages
    //     };
    //
    //     return result;
    // }
    
    public async Task<IEnumerable<JobApplication>> GetUserApplicationsForSupervisorAsync(Guid userId, Guid supervisorId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId.ToString());

        if (user == null)
            throw new KeyNotFoundException("Użytkownik nie istnieje.");

        if (user.SupervisorId != supervisorId)
            throw new AuthenticationException("Nie masz dostępu do aplikacji tego użytkownika.");

        var applications = await _context.JobApplications
            .Include(a => a.JobOffer)
            .Where(a => a.UserId == userId)
            .ToListAsync();

        return applications;
    }
    
    public async Task<IEnumerable<UserCourse>> GetUserCoursesForSupervisorAsync(Guid userId, Guid supervisorId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId.ToString());

        if (user == null)
            throw new KeyNotFoundException("Użytkownik nie istnieje.");

        if (user.SupervisorId != supervisorId)
            throw new AuthenticationException("Nie masz dostępu do kursów tego użytkownika.");

        var courses = await _context.UserCourses
            .Where(c => c.UserId == userId)
            .ToListAsync();

        return courses;
    }
    
    // public async Task<IEnumerable<WorkExperience>> GetUserWorkExperiencesForSupervisorAsync(Guid userId, Guid supervisorId)
    // {
    //     var user = await _context.Users
    //         .FirstOrDefaultAsync(u => u.Id == userId.ToString());
    //
    //     if (user == null)
    //         throw new KeyNotFoundException("Użytkownik nie istnieje.");
    //
    //     if (user.SupervisorId != supervisorId)
    //         throw new AuthenticationException("Nie masz dostępu do historii zatrudnienia tego użytkownika.");
    //
    //     var workExperiences = await _context.WorkExperiences
    //         .Include(w => w.PersonalInformation)
    //         .Where(w => w.UserId == userId)
    //         .ToListAsync();
    //
    //     foreach (var experience in workExperiences)
    //     {
    //         if (experience.PersonalInformation != null)
    //         {
    //             experience.PersonalInformation.WorkExperiences = null;
    //         }
    //     }
    //
    //     return workExperiences;
    // }

}