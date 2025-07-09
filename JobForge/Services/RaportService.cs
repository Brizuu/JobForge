using JobForge.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JobForge.Services;

public class RaportService : IRaportService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public RaportService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<int> CountUsersInCompanyAsync(Guid companyId)
    {
        return await _userManager.Users.CountAsync(u => u.CompanyId == companyId);
    }
}