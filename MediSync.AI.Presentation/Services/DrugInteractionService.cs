using MediSync.AI.Presentation.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Text.Json;

namespace MediSync.AI.Presentation.Services;

public class DrugInteractionService : IDrugInteractionService
{
    private readonly Kernel _kernel;
    private readonly ILogger<DrugInteractionService> _logger;

    public DrugInteractionService(Kernel kernel, ILogger<DrugInteractionService> logger)
    {
        _kernel = kernel;
        _logger = logger;
    }

    public async Task<DrugInteractionResult> CheckInteractionAsync(DrugInteractionRequest request, CancellationToken cancellationToken)
    {
        try
        {
            // Step 1 — Build active medications list as readable text
            var activeMedicationsText = request.ActiveMedications.Any() ? string.Join("\n", request.ActiveMedications.Select(m => $"- {m.DrugName} {m.Dosage} — {m.Frequency}")) : "None";

            // Step 2 — Build allergies list as readable text
            var allergies = request.Allergies.Any() ? string.Join("\n", request.Allergies.Select(a => $"- {a.Substance} — {a.Severity}")) : "None";

            // Step 3 — Build structured prompt
            var prompt = $$"""
                 You are a clinical pharmacist AI assistant.
                 Your job is to check drug interactions and allergies.
                 Be precise and conservative — patient safety is the priority.

                 Check if the following new prescription is safe:

                 NEW PRESCRIPTION:
                 Drug: {{request.NewDrugName}} {{request.NewDosage}}
                 Frequency: {{request.Frequency}}
                 Route: {{request.Route}}

                 PATIENT'S ACTIVE MEDICATIONS:
                 {{activeMedicationsText}}

                 PATIENT'S KNOWN ALLERGIES:
                 {{allergies}}

                 Analyze for:
                 1. Drug-drug interactions between new drug and active medications
                 2. Drug-allergy interactions between new drug and known allergies
                 3. Dosage concerns

                 Respond ONLY in this exact JSON format with no additional text:
                 {
                     "hasInteraction": true or false,
                     "severity": "Critical" or "Minor" or "None",
                     "interactions": ["list each specific interaction found"],
                     "recommendation": "specific actionable recommendation for the doctor",
                     "isSafeToDispense": true or false
                 }

                 Rules:
                 - severity "Critical" = life threatening, isSafeToDispense = false
                 - severity "Minor" = monitor required, isSafeToDispense = true
                 - severity "None" = no interactions found, isSafeToDispense = true
                 """;

            // Step 4 — Send to Groq via Semantic Kernel
            //var result = _kernel.InvokePromptAsync(prompt);
            var chatService = _kernel.GetRequiredService<IChatCompletionService>();
            var chatHistory = new ChatHistory();
            chatHistory.AddUserMessage(prompt);

            var response = await chatService.GetChatMessageContentAsync(chatHistory, cancellationToken: cancellationToken);

            var responseText = response.Content ?? string.Empty;

            _logger.LogInformation("Drug interaction check response: {Response}", responseText);

            // Step 5 — Parse JSON response
            var result = ParseResult(responseText);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Drug interaction check failed");

            // Step 6 — On failure → err on side of caution → block
            return new DrugInteractionResult(
                HasInteraction: true,
                Severity: "Critical",
                Interactions: new List<string> { "Unable to verify safety — check failed" },
                Recommendation: "AI check failed. Please verify manually before dispensing.",
                IsSafeToDispense: false);
        }
    }

    private static DrugInteractionResult ParseResult(string responseText)
    {
        var cleaned = responseText.Replace("```json", "").Replace("```", "").Trim();

        var parsed = JsonSerializer.Deserialize<DrugInteractionResult>(cleaned, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return parsed ?? SafeDefault();
    }

    private static DrugInteractionResult SafeDefault() => new DrugInteractionResult(
        HasInteraction: true,
        Severity: "Critical",
        Interactions: new List<string> { "Unable to verify safety — check failed" },
        Recommendation: "AI check failed. Please verify manually before dispensing.",
        IsSafeToDispense: false
    );
}
