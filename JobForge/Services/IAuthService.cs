namespace JobForge.Services;

public interface IAuthService
{
    Task SendEmailAsync(string toEmail, string subject, string htmlContent);
}