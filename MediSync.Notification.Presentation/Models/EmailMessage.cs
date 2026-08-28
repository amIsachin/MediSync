namespace MediSync.Notification.Presentation.Models;

public record EmailMessage(
    string To,
    string ToName,
    string Subject,
    string HtmlBody
);