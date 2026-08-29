using MediSync.BuildingBlocks.Domain;
using MediSync.MedicalRecord.Domain.Events;
using MediSync.MedicalRecord.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace MediSync.MedicalRecord.Infrastructure;

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

            if (domainEvent is PatientProfileCreatedEvent patientProfileEvent)
            {
                var payload = new
                {
                    PatientId = patientProfileEvent.Id,
                    PatientEmail = patientProfileEvent.Email,
                    PatientName = patientProfileEvent.FullName
                };

                await client.PostAsJsonAsync("/api/notification/profile-created", payload, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            // Never let notification failure break the main flow
            _logger.LogError(ex, "Failed to publish notification for {EventType}", domainEvent.GetType().Name);
        }
    }
}
