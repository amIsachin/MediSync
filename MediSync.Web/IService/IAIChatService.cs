namespace MediSync.Web.IService;

public interface IAIChatService
{
    public Task<ChatAnswerResponse> AskAsync(Guid patientId, string patientName, string message);
}


public record ChatAnswerResponse(
    string Answer,
    bool HasRelevantData
);