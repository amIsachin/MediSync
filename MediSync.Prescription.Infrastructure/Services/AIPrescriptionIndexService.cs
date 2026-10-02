using MediSync.Prescription.Application.Abstractions;
using MediSync.Prescription.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace MediSync.Prescription.Infrastructure.Services;

internal class AIPrescriptionIndexService : IAIPrescriptionIndexService
{
    private readonly IPrescriptionRepository _prescriptionRepository;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<AIPrescriptionIndexService> _logger;

    public AIPrescriptionIndexService(IPrescriptionRepository prescriptionRepository, IHttpClientFactory httpClientFactory, ILogger<AIPrescriptionIndexService> logger)
    {
        _prescriptionRepository = prescriptionRepository;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task IndexPrescriptionsAsync(Guid patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            var prescriptions = await _prescriptionRepository.GetByPatientIdAsync(patientId, cancellationToken);

            var payload = new
            {
                patientId,
                allergies = new List<object>(),
                diagnoses = new List<object>(),
                encounters = new List<object>(),
                prescriptions = prescriptions.Select(p => new
                {
                    drugName = p.Drug.DrugName,
                    dosage = p.Drug.Dosage,
                    frequency = p.Dosage.Frequency.ToString(),
                    route = p.Dosage.Route.ToString(),
                    durationDays = p.Dosage.DurationDays,
                    status = p.Status.ToString(),
                    expiresAt = p.ExpiresAt,
                    notes = p.Notes
                })
            };

            var client = _httpClientFactory.CreateClient("AIApi");
            var response = await client.PostAsJsonAsync(
                    "/api/ai/index", payload, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Prescriptions indexed for patient {PatientId}", patientId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Prescription index failed for patient {PatientId}", patientId);
        }
    }
}
