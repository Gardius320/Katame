using KatameApi.Services;

namespace KatameApi.Tests.Services;

public class FakeEmailService : IEmailService
{
    public List<(string ToEmail, string FirstName, string ResetLink)> SentPasswordResetEmails { get; } = new();

    public Task SendPasswordResetEmailAsync(string toEmail, string firstName, string resetLink)
    {
        SentPasswordResetEmails.Add((toEmail, firstName, resetLink));
        return Task.CompletedTask;
    }
}
