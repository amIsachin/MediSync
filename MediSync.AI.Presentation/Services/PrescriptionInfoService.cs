using MediSync.AI.Presentation.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Text.Json;

namespace MediSync.AI.Presentation.Services;

public class PrescriptionInfoService : IPrescriptionInfoService
{
    private readonly Kernel _kernel;
    private readonly ILogger<PrescriptionInfoService> _logger;

    public PrescriptionInfoService(Kernel kernel, ILogger<PrescriptionInfoService> logger)
    {
        _kernel = kernel;
        _logger = logger;
    }

    public async Task<PrescriptionInfoResponse> GetInfoAsync(PrescriptionInfoRequest request, CancellationToken cancellationToken = default)
    {
        var specificQuestion = string.IsNullOrEmpty(request.PatientQuestion) ? string.Empty : $"\nPatient's specific question: {request.PatientQuestion}";

        // Build prompt as plain string — no SK template syntax
        var prompt = "You are a patient education specialist explaining medications " +
                     "in simple, easy to understand language.\n\n" +
                     $"Explain this medication to a patient:\n" +
                     $"Drug: {request.DrugName} {request.Dosage}\n" +
                     $"{specificQuestion}\n\n" +
                     "Respond ONLY in this exact JSON format with no extra text:\n" +
                     "{\n" +
                     "  \"drugName\": \"the drug name\",\n" +
                     "  \"purpose\": \"what this medication is used for\",\n" +
                     "  \"howToTake\": \"simple instructions on how to take it\",\n" +
                     "  \"commonSideEffects\": \"most common side effects\",\n" +
                     "  \"whenToCallDoctor\": \"symptoms requiring doctor call\",\n" +
                     "  \"importantWarnings\": \"most important warnings\"\n" +
                     "}\n\n" +
                     "Rules:\n" +
                     "- Use simple everyday language\n" +
                     "- Be reassuring but honest\n" +
                     "- Keep each section 2-3 sentences maximum";

        var history = new ChatHistory();
        history.AddUserMessage(prompt);

        var chatService = _kernel.GetRequiredService<IChatCompletionService>();

        var result = await chatService.GetChatMessageContentAsync(history, cancellationToken: cancellationToken);

        var responseText = result.ToString();

        // Clean response
        var cleaned = responseText.Replace("```json", "").Replace("```", "").Trim();

        var info = JsonSerializer.Deserialize<PrescriptionInfoResponse>(cleaned, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return info ?? DefaultResponse(request.DrugName);
    }

    private static PrescriptionInfoResponse DefaultResponse(string drugName) => new(
        DrugName: drugName,
        Purpose: "Information unavailable. Please consult your doctor.",
        HowToTake: "Follow your doctor's instructions.",
        CommonSideEffects: "Please consult your doctor or pharmacist.",
        WhenToCallDoctor: "If you experience any unusual symptoms.",
        ImportantWarnings: "Always follow your doctor's advice."
    );
}
