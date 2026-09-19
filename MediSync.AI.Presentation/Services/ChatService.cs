using MediSync.AI.Presentation.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace MediSync.AI.Presentation.Services;

public class ChatService : IChatService
{
    private readonly Kernel _kernel;
    private readonly IEmbeddingService _embeddingService;
    private readonly IVectorStoreService _vectorStoreService;
    private readonly ILogger<ChatService> _logger;

    public ChatService(Kernel kernel, IEmbeddingService embeddingService, IVectorStoreService vectorStoreService, ILogger<ChatService> logger)
    {
        _kernel = kernel;
        _embeddingService = embeddingService;
        _vectorStoreService = vectorStoreService;
        _logger = logger;
    }

    public async Task<ChatResponse> AskAsync(ChatRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Chat request from patient {PatientId}: {Message}", request.PatientId, request.Message);

        // Embed the question
        var queryVector = await _embeddingService.EmbedQueryAsync(request.Message, cancellationToken);

        if (queryVector.Length == 0)
        {
            return new ChatResponse(
                Answer: "I am unable to process your question right now. Please try again.",
                SourceRecords: new List<string>(),
                HasRelevantData: false
            );
        }

        // Search Qdrant for relevant chunk
        var relevantChunks = await _vectorStoreService.SearchAsync(queryVector, request.PatientId, topK: 5, cancellationToken);

        _logger.LogInformation("Found {Count} relevant chunks for patient {PatientId}", relevantChunks.Count, request.PatientId);

        // Build context from chunks

        var hasData = relevantChunks.Any();

        var context = hasData
                ? string.Join("\n\n", relevantChunks.Select((c, i) => $"Record {i + 1} ({c.RecordType}): {c.Content}"))
                : "No relevant health records found for this question.";

        // Build chat history
        var chatService = _kernel.GetRequiredService<IChatCompletionService>();

        var history = new ChatHistory();

        // System message — rules for AI
        history.AddSystemMessage($"""
                You are a personal health assistant for {request.PatientName}.
                Your job is to answer questions about their health records
                in simple, clear, non-medical language.

                Rules:
                - Only answer from the provided health records
                - Never provide medical diagnoses or treatment advice
                - Always recommend consulting their doctor for medical decisions
                - Use simple language — avoid complex medical terms
                - Be empathetic and supportive
                - If records do not contain relevant information — say so honestly
                - Keep answers concise and easy to understand
                """);

        // User message with context + question
        history.AddUserMessage($"""
                My health records relevant to your question:
                {context}

                My question: {request.Message}
                """);

        var response = await chatService.GetChatMessageContentAsync(history, cancellationToken: cancellationToken);

        var answer = response.Content ?? "I could not generate an answer. Please try again.";

        return new ChatResponse(
            Answer: answer,
            SourceRecords: relevantChunks.Select(c => c.RecordType).Distinct().ToList(),
            HasRelevantData: hasData
        );
    }
}