using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using JobForge.Data;
using JobForge.DbModels;
using JobForge.Models;
using JobForge.Services;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MimeKit;

namespace JobForge.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly AppDbContext _context;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly AuthService _authService;
    public AuthController(UserManager<ApplicationUser> userManager, IConfiguration configuration, AppDbContext context, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _configuration = configuration;
        _context = context;
        _roleManager = roleManager;
        _authService = new AuthService(configuration);
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

    if (!Guid.TryParse(user.Id, out Guid userGuid))
    {
        // Obsłuż błąd, jeśli nie da się przekonwertować
        throw new Exception("User.Id is not a valid GUID.");
    }
    
    // ✅ DODAJEMY GENEROWANE CV
    var generatedCV = new GeneratedCV
    {
        UserId = userGuid,
        GenerationDate = DateTime.UtcNow
    };
    _context.GeneratedCVs.Add(generatedCV);
    await _context.SaveChangesAsync(); // zapisz, by mieć ID

    // ✅ DODAJEMY DANE PERSONALNE
    var personalInfo = new PersonalInformation
    {
        UserId = userGuid,
        FirstName = user.FirstName,
        LastName = user.LastName,
        EmailAddress = user.Email,
        GeneratedCVId = generatedCV.Id
    };
    _context.PersonalInformations.Add(personalInfo);
    await _context.SaveChangesAsync();

    // 🔐 GENEROWANIE TOKENU I WYSYŁKA MAILA
    var tokenHandler = new JwtSecurityTokenHandler();
    var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(new[]
        {
            new Claim("userId", user.Id),
            new Claim("tokenType", "email_confirmation")
        }),
        Expires = DateTime.UtcNow.AddHours(24),
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    };

    var token = tokenHandler.CreateToken(tokenDescriptor);
    var tokenString = tokenHandler.WriteToken(token);

    var confirmationLink = $"{Request.Scheme}://{Request.Host}/api/auth/confirm-email?token={tokenString}";

    var emailBody = $"<h3>Witaj {user.FirstName}, potwierdź swoje konto klikając poniższy link:</h3>" +
                    $"<p><a href='{confirmationLink}'>Potwierdź konto</a></p>";

    await _authService.SendEmailAsync(user.Email, "Potwierdzenie konta JobForge", emailBody);

    return Ok(new { message = "User registered, assigned to 'free' role, CV and personal info created, confirmation mail sent" });
}




    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            return Unauthorized();

        var roles = await _userManager.GetRolesAsync(user); 

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


    [Authorize]
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

        var roles = await _userManager.GetRolesAsync(user); 

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
    
    [Authorize(Roles = "Admin")]
    [HttpPost("assign-premium/{userId}")]
    public async Task<IActionResult> AssignPremiumRole(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return NotFound(new { message = "Nie znaleziono użytkownika o podanym Id." });
        }

    
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
    
    
    [HttpPost("send-confirmation-email/{userId}")]
    public async Task<IActionResult> SendConfirmationEmail(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound("Nie znaleziono użytkownika.");

     
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("userId", user.Id),
                new Claim("tokenType", "email_confirmation")
            }),
            Expires = DateTime.UtcNow.AddHours(24),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        var confirmationLink = $"{Request.Scheme}://{Request.Host}/api/auth/confirm-email?token={tokenString}";

        var emailBody = $"<h3>Potwierdź swoje konto klikając w link:</h3><p><a href='{confirmationLink}'>Potwierdź konto</a></p>";

        await _authService.SendEmailAsync(user.Email, "Potwierdzenie konta JobForge", emailBody);

        return Ok("Link potwierdzający został wysłany na maila.");
    }

    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

        try
        {
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var tokenType = principal.Claims.FirstOrDefault(c => c.Type == "tokenType")?.Value;
            if (tokenType != "email_confirmation")
                return BadRequest("Nieprawidłowy token.");

            var userIdClaim = principal.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
            if (userIdClaim == null)
                return BadRequest("Nieprawidłowy token.");

            var user = await _userManager.FindByIdAsync(userIdClaim);
            if (user == null)
                return NotFound("Użytkownik nie istnieje.");
            
            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);

            return Ok("Konto zostało pomyślnie potwierdzone.");
        }
        catch (Exception)
        {
            return BadRequest("Token jest nieprawidłowy lub wygasł.");
        }
    }
    
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
            return Ok(new { message = "Jeśli podany email jest zarejestrowany, otrzymasz link do resetu hasła." });

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        var resetLink = Url.Action(nameof(ResetPassword), "Auth", new { token, email = user.Email }, Request.Scheme);

        var emailBody = $"Kliknij w link, aby zresetować hasło: <a href='{resetLink}'>Resetuj hasło</a>";

        await _authService.SendEmailAsync(user.Email, "Resetowanie hasła", emailBody);

        return Ok(new { message = "Jeśli podany email jest zarejestrowany, otrzymasz link do resetu hasła." });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
            return BadRequest(new { message = "Nie znaleziono użytkownika." });

        var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok(new { message = "Hasło zostało zmienione pomyślnie." });
    }


}
