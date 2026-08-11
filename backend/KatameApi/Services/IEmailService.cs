namespace KatameApi.Services;

public interface IEmailService
{
    Task SendPasswordResetEmailAsync(string toEmail, string firstName, string resetLink);
}
