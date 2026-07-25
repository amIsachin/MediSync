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

        return ServiceResponseMessage<Guid>.Failure(response.StatusCode.ToString(), "Failed to create patient profile.", "Failed");

    }

    public async Task<ServiceResponseMessage<Guid>> GetPatientByUserIdAsync(Guid userId)
    {
        var response = await _httpClient.GetAsync($"https://localhost:7000/api/patients/user/{userId}");

        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadFromJsonAsync<Guid>(new JsonSerializerOptions { PropertyNameCaseInsensitive = false });

            return ServiceResponseMessage<Guid>.Success(data);
        }

        var error = await response.Content.ReadAsStringAsync();
        var errorMessage = MedicalRecordService.ExtractMessage(error) ?? "Failed to load patient profile.";

        return ServiceResponseMessage<Guid>.Failure(response.StatusCode.ToString(), "Failed to load patient profile.", "Failed");
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
