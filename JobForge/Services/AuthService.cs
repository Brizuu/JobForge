using MailKit.Net.Smtp;
using MimeKit;

namespace JobForge.Services;

public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;

    public AuthService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string htmlContent)
    {
        var emailSettings = _configuration.GetSection("EmailSettings");
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(emailSettings["FromName"], emailSettings["FromEmail"]));
        email.To.Add(MailboxAddress.Parse(toEmail));
        email.Subject = subject;

        var builder = new BodyBuilder { HtmlBody = htmlContent };
        email.Body = builder.ToMessageBody();

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(emailSettings["SmtpHost"], int.Parse(emailSettings["SmtpPort"]), MailKit.Security.SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(emailSettings["SmtpUser"], emailSettings["SmtpPass"]);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}