using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace KatameApi.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailSettings> settings, ILogger<EmailService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task SendPasswordResetEmailAsync(string toEmail, string firstName, string resetLink)
    {
        // Todo el envío (incluida la construcción del mensaje) va dentro del try: si
        // faltan credenciales SMTP configuradas o el envío falla por cualquier motivo,
        // el flujo de "olvidé mi contraseña" no debe romperse ni filtrar si el correo
        // existe o no según el status code de la respuesta al cliente.
        try
        {
            if (string.IsNullOrWhiteSpace(_settings.SenderEmail) || string.IsNullOrWhiteSpace(_settings.SenderPassword))
            {
                _logger.LogWarning(
                    "Email:SenderEmail / Email:SenderPassword no están configurados (User Secrets). No se envió el correo de recuperación a {Email}.",
                    toEmail);
                return;
            }

            var body = $"""
                <p>Hola {firstName},</p>
                <p>Recibimos una solicitud para restablecer tu contraseña de Katame.</p>
                <p><a href="{resetLink}">Hacé clic acá para crear una nueva contraseña</a>. El enlace vence en 30 minutos.</p>
                <p>Si no pediste esto, podés ignorar este correo — tu contraseña actual sigue funcionando normalmente.</p>
                """;

            using var message = new MailMessage
            {
                From = new MailAddress(_settings.SenderEmail, _settings.SenderName),
                Subject = "Recuperá tu contraseña de Katame",
                Body = body,
                IsBodyHtml = true,
            };
            message.To.Add(toEmail);

            using var client = new SmtpClient(_settings.SmtpHost, _settings.SmtpPort)
            {
                Credentials = new NetworkCredential(_settings.SenderEmail, _settings.SenderPassword),
                EnableSsl = true,
            };

            await client.SendMailAsync(message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "No se pudo enviar el correo de recuperación de contraseña a {Email}", toEmail);
        }
    }
}
