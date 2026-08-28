using MediSync.Notification.Presentation.Models;
using System.Net;
using System.Net.Mail;

namespace MediSync.Notification.Presentation.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendAsync(EmailMessage emailMessage, CancellationToken cancellationToken = default)
    {
        try
        {
            using var message = new MailMessage
            {
                From = new MailAddress(_configuration["EmailSettings:SenderEmail"]!, _configuration["EmailSettings:SenderName"]),
                Subject = emailMessage.Subject,
                Body = emailMessage.HtmlBody,
                IsBodyHtml = true
            };

            message.To.Add(emailMessage.To);

            using var smtpClient = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(_configuration["EmailSettings:SmtpUser"]!, _configuration["EmailSettings:SmtpPassword"]!)
            };

            cancellationToken.ThrowIfCancellationRequested();

            await smtpClient.SendMailAsync(message, cancellationToken);

            _logger.LogInformation("Email sent to {Email} — Subject: {Subject}", message.To, message.Subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}", emailMessage.To);

            // Do not throw — notification failure should not break the main flow
        }
    }
}
