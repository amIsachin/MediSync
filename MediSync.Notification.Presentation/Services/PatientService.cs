using MediSync.BuildingBlocks.Common;
using System.Text.Json;

namespace MediSync.Notification.Presentation.Services;

public class PatientService : IPatientService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<PatientService> _logger;

    public PatientService(IHttpClientFactory httpClientFactory, ILogger<PatientService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<Result<PatientContactInfo>> GetPatientContactAsync(Guid patientId, CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient("MedicalRecordApi");

        var response = await client.GetAsync($"/medical/Patient/GetById/{patientId}", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return Result<PatientContactInfo>.Failure(Error.NotFound(response.StatusCode.ToString(), "Patient not found"));
        }

        var data = await response.Content.ReadFromJsonAsync<PatientContactInfo>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return Result<PatientContactInfo>.Success(data!);
    }
}


public record PatientContactResponse(
    Guid Id,
    string FullName,
    string Email
);