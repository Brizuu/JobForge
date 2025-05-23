using Microsoft.AspNetCore.Identity;

namespace JobForge.Models;

public class ApplicationUser : IdentityUser
{
    // public List<RefreshToken> RefreshTokens { get; set; } = new();
    public string FirstName { get; set; }
    public string LastName { get; set; }
    // public int Credits { get; set; } = 0;
}