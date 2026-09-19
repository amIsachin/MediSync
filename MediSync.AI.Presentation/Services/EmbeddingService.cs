using System.Text.Json;

namespace MediSync.AI.Presentation.Services;

public class EmbeddingService : IEmbeddingService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<EmbeddingService> _logger;

    public EmbeddingService(HttpClient httpClient, ILogger<EmbeddingService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<float[]> EmbedDocumentAsync(string texts, CancellationToken cancellationToken = default)
        => await EmbedAsync(texts, "search_document", cancellationToken);

    public async Task<float[]> EmbedQueryAsync(string text, CancellationToken cancellationToken = default)
        => await EmbedAsync(text, "search_query", cancellationToken);

    private async Task<float[]> EmbedAsync(string texts, string inputType, CancellationToken cancellationToken = default)
    {
        try
        {
            var payload = new
            {
                texts = new[] { texts },
                model = "embed-english-v3.0",
                input_Type = inputType
            };

            var response = await _httpClient.PostAsJsonAsync("https://api.cohere.ai/v1/embed", payload, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to get embedding. Status code: {StatusCode}, Reason: {ReasonPhrase}", response.StatusCode, response.ReasonPhrase);
                return Array.Empty<float>();
            }

            var result = await response.Content.ReadFromJsonAsync<CohereEmbedResponse>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true }, cancellationToken);

            return result?.Embeddings![0] ?? Array.Empty<float>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting embedding.");
            return Array.Empty<float>();
        }
    }
}


// Cohere API response model
public class CohereEmbedResponse
{
    public float[][]? Embeddings { get; set; }
}