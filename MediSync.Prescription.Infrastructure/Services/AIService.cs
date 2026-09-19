using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;

namespace MediSync.Prescription.Infrastructure.Services;

public class AIService : IAIService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<AIService> _logger;

    public AIService(IHttpClientFactory httpClientFactory, ILogger<AIService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<DrugInteractionCheckResult> CheckDrugInteractionAsync(DrugInteractionCheckRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("AIApi");
            var response = await client.PostAsJsonAsync("/api/ai/drug-interaction", request, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                // HTTP 200 — safe to dispense
                var result = await response.Content.ReadFromJsonAsync<DrugInteractionCheckResult>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true }, cancellationToken);

                return result!;
            }

            if ((int)response.StatusCode == 409)
            {
                // HTTP 409 — blocked
                var result = await response.Content.ReadFromJsonAsync<DrugInteractionCheckResult>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true }, cancellationToken);

                return result!;
            }

            // Unexpected error — err on side of caution
            _logger.LogWarning("AI API returned unexpected status: {Status}", response.StatusCode);

            return SafeDefault();
        }
        catch (Exception ex)
        {// AI service unavailable — do not block prescription
            // Log and allow — better than blocking all prescriptions
            _logger.LogError(ex, "AI service unavailable — skipping interaction check");
            return SafeDefault();
        }
    }

    private static DrugInteractionCheckResult SafeDefault() => new(
        Blocked: false,
        Severity: "Unknown",
        Interactions: new List<string>(),
        Recommendation: "AI check unavailable — verify manually",
        IsSafeToDispense: true   // fail open — do not block when AI is down
    );
}
