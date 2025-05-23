using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using JobForge.Data;
using JobForge.DbModels;
using JobForge.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace JobForge.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly AppDbContext _context;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AuthController(UserManager<ApplicationUser> userManager, IConfiguration configuration, AppDbContext context, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _configuration = configuration;
        _context = context;
        _roleManager = roleManager;
    }
    
    private RefreshToken GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return new RefreshToken
        {
            Token = Convert.ToBase64String(randomBytes),
            Expires = DateTime.UtcNow.AddDays(7)
        };
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
        };

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
            return BadRequest(result.Errors);
        
        const string defaultRole = "free";

        if (!await _roleManager.RoleExistsAsync(defaultRole))
        {
            await _roleManager.CreateAsync(new IdentityRole(defaultRole));
        }

        await _userManager.AddToRoleAsync(user, defaultRole);

        return Ok(new { message = "User registered and assigned to 'free' role" });
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            return Unauthorized();

        var roles = await _userManager.GetRolesAsync(user); // ⬅️ Pobierz role

        var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        
        foreach (var role in roles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, role));
        }

        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            expires: DateTime.Now.AddMinutes(15),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        var refreshToken = GenerateRefreshToken();
        refreshToken.UserId = user.Id;

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        Response.Cookies.Append("access_token", new JwtSecurityTokenHandler().WriteToken(token), new CookieOptions
        {
            HttpOnly = true,
            Expires = token.ValidTo,
            Secure = true,
            SameSite = SameSiteMode.Strict
        });

        Response.Cookies.Append("refresh_token", refreshToken.Token, new CookieOptions
        {
            HttpOnly = true,
            Expires = refreshToken.Expires,
            Secure = true,
            SameSite = SameSiteMode.Strict
        });

        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token),
            expiration = token.ValidTo,
            refreshToken = refreshToken.Token
        });
    }


    
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var refreshTokenFromCookies = Request.Cookies["refresh_token"];

        if (string.IsNullOrEmpty(refreshTokenFromCookies))
            return Unauthorized("Refresh token is missing or expired.");

        var storedToken = await _context.RefreshTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Token == refreshTokenFromCookies);

        if (storedToken == null || !storedToken.IsActive)
            return Unauthorized("Invalid or expired refresh token.");

        storedToken.Revoked = DateTime.UtcNow;

        var user = storedToken.User;

        var roles = await _userManager.GetRolesAsync(user); // ⬅️ Pobierz role

        var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var role in roles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, role)); // ⬅️ Dodaj role
        }

        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

        var newAccessToken = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            expires: DateTime.Now.AddMinutes(15),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        var accessTokenString = new JwtSecurityTokenHandler().WriteToken(newAccessToken);

        var newRefreshToken = GenerateRefreshToken();
        newRefreshToken.UserId = user.Id;

        _context.RefreshTokens.Add(newRefreshToken);
        await _context.SaveChangesAsync();

        Response.Cookies.Append("access_token", accessTokenString, new CookieOptions
        {
            HttpOnly = true,
            Expires = newAccessToken.ValidTo,
            Secure = true,
            SameSite = SameSiteMode.Strict
        });

        Response.Cookies.Append("refresh_token", newRefreshToken.Token, new CookieOptions
        {
            HttpOnly = true,
            Expires = newRefreshToken.Expires,
            Secure = true,
            SameSite = SameSiteMode.Strict
        });

        return Ok(new
        {
            access_token = accessTokenString,
            access_token_expires = newAccessToken.ValidTo,
            refresh_token = newRefreshToken.Token,
            refresh_token_expires = newRefreshToken.Expires
        });
    }
    
    [HttpPost("assign-premium/{userId}")]
    public async Task<IActionResult> AssignPremiumRole(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return NotFound(new { message = "Nie znaleziono użytkownika o podanym Id." });
        }

        // Sprawdzenie, czy użytkownik już ma rolę Premium
        if (await _userManager.IsInRoleAsync(user, "Premium"))
        {
            return BadRequest(new { message = "Użytkownik już posiada rangę Premium." });
        }

        var result = await _userManager.AddToRoleAsync(user, "Premium");
        if (result.Succeeded)
        {
            return Ok(new { message = "Ranga Premium została nadana użytkownikowi." });
        }
        else
        {
            return StatusCode(500, new { message = "Nie udało się nadać rangi Premium.", errors = result.Errors });
        }
    }


}
