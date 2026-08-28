using MediSync.Web.IService;
using MediSync.Web.Services.ServiceResponse;
using System.Text.Json;

namespace MediSync.Web.Services;

public class PrescriptionService : IPrescriptionService
{
    private readonly HttpClient _httpClient;

    public PrescriptionService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ServiceResponseMessage<bool>> CancelPrescriptionAsync(Guid prescriptionId, string reason)
    {
        var response = await _httpClient.PostAsJsonAsync($"https://localhost:7000/patients/cancel/{prescriptionId}", new { reason });

        if (response.IsSuccessStatusCode)
        {
            return ServiceResponseMessage<bool>.Success(true);
        }

        var errorMessage = await response.Content.ReadAsStringAsync();
        return ServiceResponseMessage<bool>.Failure(response.StatusCode.ToString(), errorMessage, "Failed");
    }

    public async Task<ServiceResponseMessage<Guid>> CreatePrescriptionAsync(CreatePrescriptionRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync($"https://localhost:7000/patients/Create", request);

        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadFromJsonAsync<Guid>();

            return ServiceResponseMessage<Guid>.Success(data);
        }

        var errorMessage = await response.Content.ReadAsStringAsync();
        return ServiceResponseMessage<Guid>.Failure(response.StatusCode.ToString(), errorMessage, "Failed");
    }

    public async Task<ServiceResponseMessage<List<PrescriptionResponse>>> GetPatientPrescriptionsAsync(Guid patientId, bool isActiveOnly = false)
    {
        var activeOnly = string.Empty;

        if (isActiveOnly is true)
        {
            activeOnly = "?activeOnly=true";
        }

        _httpClient.Timeout = TimeSpan.FromMinutes(10);
        var response = await _httpClient.GetAsync($"https://localhost:7000/patients/{patientId}{activeOnly}");

        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadFromJsonAsync<List<PrescriptionResponse>>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return ServiceResponseMessage<List<PrescriptionResponse>>.Success(data ?? null!);
        }

        var errorMessage = await response.Content.ReadAsStringAsync();
        return ServiceResponseMessage<List<PrescriptionResponse>>.Failure(response.StatusCode.ToString(), errorMessage, "Failed to retrieve patient prescriptions.");
    }
}
