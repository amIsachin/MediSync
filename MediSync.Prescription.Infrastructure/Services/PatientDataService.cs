using MediSync.Prescription.Application.Abstractions;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace MediSync.Prescription.Infrastructure.Services;

public class PatientDataService : IPatientDataService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<PatientDataService> _logger;

    public PatientDataService(IHttpClientFactory httpClientFactory, ILogger<PatientDataService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<List<PatientAllergyInfo>> GetPatientAllergiesAsync(Guid patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("Gateway");
            var response = await client.GetAsync($"/medical/Patient/GetById/{patientId}", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to retrieve patient data for patientId {PatientId}. Status Code: {StatusCode}", patientId, response.StatusCode);
                return new List<PatientAllergyInfo>();
            }

            var patient = await response.Content.ReadFromJsonAsync<PatientProfileResponse>(new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }, cancellationToken);

            if (patient?.Allergies is null)
            {
                return new List<PatientAllergyInfo>();
            }

            var allergies = patient.Allergies.Select(a => new PatientAllergyInfo
            (
                a.Substance,
                a.Severity
            )).ToList();

            return allergies;
        }
        catch (Exception ex)
        {
            // Never let allergy fetch failure block prescription flow
            _logger.LogError(ex, "Failed to fetch allergies for patient {PatientId}", patientId);
            return new List<PatientAllergyInfo>();
        }
    }
}

// Minimal response models — only what we need
public record PatientProfileResponse(
    Guid Id,
    List<AllergyResponse> Allergies
);

public record AllergyResponse(
    string Substance,
    string Severity
);