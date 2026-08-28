using MediSync.BuildingBlocks.Domain;
using MediSync.Prescription.Domain.Events;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace MediSync.Prescription.Infrastructure;

public class NotificationPublisher : INotificationPublisher
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<NotificationPublisher> _logger;

    public NotificationPublisher(IHttpClientFactory httpClientFactory, ILogger<NotificationPublisher> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task PublishAsync(IDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("NotificationApi");

            if (domainEvent is PrescriptionCreatedEvent prescriptionEvent)
            {
                var payload = new
                {
                    prescriptionEvent.PrescriptionId,
                    prescriptionEvent.PatientId,
                    FullName = string.Empty,
                    prescriptionEvent.DrugName,
                    prescriptionEvent.Dosage,
                    Frequency = string.Empty,
                    DurationDays = string.Empty,
                    PrescribedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddDays(30)
                };

                await client.PostAsJsonAsync("/api/notification/prescription-created", payload, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            // Never let notification failure break the main flow
            _logger.LogError(ex, "Failed to publish notification for {EventType}",
                domainEvent.GetType().Name);
        }
    }
}
