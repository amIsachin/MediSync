namespace MediSync.AI.Presentation.Services;

public interface IEmbeddingService
{
    // Used when STORING records into Qdrant
    Task<float[]> EmbedDocumentAsync(string text, CancellationToken cancellationToken = default);


    // Used when SEARCHING — patient question
    Task<float[]> EmbedQueryAsync(string texts, CancellationToken cancellationToken = default);
}
