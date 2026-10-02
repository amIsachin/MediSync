namespace MediSync.AI.Presentation.Services;

public interface ISummaryService
{
    Task<SummaryResponse> SummarizeAsync(Guid patientId, string patientName, CancellationToken cancellationToken = default);
}

public record SummaryResponse(
    string Summary,
    int TotalRecords,
    bool HasData
);
