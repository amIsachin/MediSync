using MediSync.Web.IService;
using MediSync.Web.Services.ServiceResponse;
using System.Text.Json;

namespace MediSync.Web.Services;

public class MedicalRecordService : IMedicalRecordService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<MedicalRecordService> _logger;

    public MedicalRecordService(HttpClient httpClient, ILogger<MedicalRecordService> logger)
    {
        _httpClient = httpClient;
        _httpClient.Timeout = TimeSpan.FromMinutes(10);
        _logger = logger;
    }

    public async Task<ServiceResponseMessage<Guid>> CreatePatientProfileAsync(CreatePatientProfileRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("https://localhost:7000/medical/patient", request);

        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadFromJsonAsync<Guid>(new JsonSerializerOptions { PropertyNameCaseInsensitive = false });

            return ServiceResponseMessage<Guid>.Success(data);
        }

        var error = await response.Content.ReadAsStringAsync();
        var errorMessage = MedicalRecordService.ExtractMessage(error) ?? "Failed to create patient profile.";

        return ServiceResponseMessage<Guid>.Failure(Convert.ToInt32(response.StatusCode).ToString(), errorMessage, "Failed");
    }


    public async Task<ServiceResponseMessage<PatientProfileResponse>> GetPatientByUserIdAsync(Guid userId)
    {
        var response = await _httpClient.GetAsync($"https://localhost:7000/patients/user/{userId}");

        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadFromJsonAsync<PatientProfileResponse>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return ServiceResponseMessage<PatientProfileResponse>.Success(data!);
        }

        var error = await response.Content.ReadAsStringAsync();
        var errorMessage = MedicalRecordService.ExtractMessage(error) ?? "Failed to load patient profile.";

        return ServiceResponseMessage<PatientProfileResponse>.Failure(response.StatusCode.ToString(), errorMessage, "Failed");
    }

    public async Task<ServiceResponseMessage<PatientProfileResponse>> GetPatientByEmailAsync(string email)
    {
        var response = await _httpClient.GetAsync($"https://localhost:7000/medical/Patient/email/{email}");

        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadFromJsonAsync<PatientProfileResponse>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return ServiceResponseMessage<PatientProfileResponse>.Success(data!);
        }

        var error = await response.Content.ReadAsStringAsync();
        var errorMessage = MedicalRecordService.ExtractMessage(error) ?? "Failed to load patient profile.";

        return ServiceResponseMessage<PatientProfileResponse>.Failure(response.StatusCode.ToString(), errorMessage, "Failed");
    }

    public async Task<ServiceResponseMessage<PatientProfileResponse>> GetPatientByIdAsync(Guid id)
    {
        var response = await _httpClient.GetAsync($"https://localhost:7000/medical/patient/GetById/{id}");

        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadFromJsonAsync<PatientProfileResponse>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return ServiceResponseMessage<PatientProfileResponse>.Success(data!);
        }

        var error = await response.Content.ReadAsStringAsync();
        var errorMessage = MedicalRecordService.ExtractMessage(error) ?? "Failed to load patient profile.";

        return ServiceResponseMessage<PatientProfileResponse>.Failure(response.StatusCode.ToString(), errorMessage, "Failed");
    }

    public async Task<ServiceResponseMessage<bool>> AddAllergyAsync(Guid patientId, AddAllergyRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync($"https://localhost:7000/medical/patient/{patientId}/allergy", request);

        if (response.IsSuccessStatusCode)
        {
            return ServiceResponseMessage<bool>.Success(true);
        }

        var error = await response.Content.ReadAsStringAsync();
        var errorMessage = MedicalRecordService.ExtractMessage(error) ?? "Failed to add allergy.";

        return ServiceResponseMessage<bool>.Failure(response.StatusCode.ToString(), errorMessage, "Failed");
    }

    public async Task<ServiceResponseMessage<bool>> AddDiagnosisAsync(Guid patientId, AddDiagnosisRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync($"https://localhost:7000/medical/patient/{patientId}/diagnoses", request);

        if (response.IsSuccessStatusCode)
        {
            return ServiceResponseMessage<bool>.Success(true);
        }

        var error = await response.Content.ReadAsStringAsync();
        var errorMessage = MedicalRecordService.ExtractMessage(error) ?? "Failed to add allergy.";

        return ServiceResponseMessage<bool>.Failure(response.StatusCode.ToString(), errorMessage, "Failed");
    }

    public async Task<ServiceResponseMessage<bool>> RecordEncounterAsync(Guid patientId, RecordEncounterRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync($"https://localhost:7000/medical/patient/{patientId}/encounters", request);

        if (response.IsSuccessStatusCode)
        {
            return ServiceResponseMessage<bool>.Success(true);
        }

        var error = await response.Content.ReadAsStringAsync();
        var errorMessage = MedicalRecordService.ExtractMessage(error) ?? "Failed to add allergy.";

        return ServiceResponseMessage<bool>.Failure(response.StatusCode.ToString(), errorMessage, "Failed");
    }

    private static string ExtractMessage(string body)
    {
        try
        {
            using (var doc = JsonDocument.Parse(body))
            {
                if (doc.RootElement.TryGetProperty("message", out var message))
                {
                    return message.GetString() ?? string.Empty;
                }

                if (doc.RootElement.TryGetProperty("Message", out var message1))
                {
                    return message1.GetString() ?? string.Empty;
                }

                return default!;
            }
        }
        catch (Exception)
        {
            return default!;
        }
    }
}
