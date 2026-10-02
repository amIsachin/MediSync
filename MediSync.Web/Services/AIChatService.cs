using MediSync.Web.IService;
using System.Text.Json;

namespace MediSync.Web.Services;

public class AIChatService : IAIChatService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<AIChatService> _logger;

    public AIChatService(IHttpClientFactory httpClientFactory, ILogger<AIChatService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<ChatAnswerResponse> AskAsync(Guid patientId, string patientName, string message)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("Gateway");
            var payload = new { patientId, patientName, message };


            var response = await client.PostAsJsonAsync("/ai/chat", payload);

            if (!response.IsSuccessStatusCode)
            {
                return new ChatAnswerResponse("I could not process your question. Please try again.", false);
            }

            var result = await response.Content.ReadFromJsonAsync<AIChatApiResponse>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return new ChatAnswerResponse(result?.Answer ?? "No answer returned.", result?.HasRelevantData ?? false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AI chat failed");
            return new ChatAnswerResponse("I am currently unavailable. Please try again later.", false);
        }
    }
}


public record AIChatApiResponse(
    string Answer,
    List<string> SourceRecords,
    bool HasRelevantData
);