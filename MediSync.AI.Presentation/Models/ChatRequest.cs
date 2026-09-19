namespace MediSync.AI.Presentation.Models;

public record ChatRequest(
    Guid PatientId,
    string Message,
    string PatientName
);

public record ChatResponse(
    string Answer,
    List<string> SourceRecords,
    bool HasRelevantData
);