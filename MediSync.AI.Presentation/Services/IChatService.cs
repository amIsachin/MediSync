using MediSync.AI.Presentation.Models;

namespace MediSync.AI.Presentation.Services;

public interface IChatService
{
    Task<ChatResponse> AskAsync(ChatRequest request, CancellationToken cancellationToken = default);
}
