using MediSync.Notification.Presentation.Models;

namespace MediSync.Notification.Presentation.Services;

public interface IEmailService
{
    Task SendAsync(EmailMessage emailMessage, CancellationToken cancellationToken = default);
}
