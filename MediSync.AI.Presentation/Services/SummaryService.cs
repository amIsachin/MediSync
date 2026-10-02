using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace MediSync.AI.Presentation.Services;

public class SummaryService : ISummaryService
{
    private readonly Kernel _kernel;
    private readonly IVectorStoreService _vectorStoreService;
    private readonly ILogger<SummaryService> _logger;

    public SummaryService(Kernel kernel, IVectorStoreService vectorStoreService, ILogger<SummaryService> logger)
    {
        _kernel = kernel;
        _vectorStoreService = vectorStoreService;
        _logger = logger;
    }

    public async Task<SummaryResponse> SummarizeAsync(Guid patientId, string patientName, CancellationToken cancellationToken = default)
    {
        try
        {
            var allRecords = await _vectorStoreService.GetAllPatientRecordsAsync(patientId, cancellationToken);

            if (!allRecords.Any())
            {
                return new SummaryResponse(Summary: "No medical records found for this patient.", TotalRecords: 0, HasData: false);
            }

            var allergies = allRecords.Where(r => r.RecordType == "allergy").ToList();
            var diagnoses = allRecords.Where(r => r.RecordType == "diagnosis").ToList();
            var encounters = allRecords.Where(r => r.RecordType == "encounter").ToList();
            var prescriptions = allRecords.Where(r => r.RecordType == "prescription").ToList();

            var context = $"""
                PATIENT: {patientName}

                ALLERGIES ({allergies.Count}):
                {(allergies.Any() ? string.Join("\n", allergies.Select(a => $"- {a.Content}")) : "None recorded")}

                DIAGNOSES ({diagnoses.Count}):
                {(diagnoses.Any() ? string.Join("\n", diagnoses.Select(d => $"- {d.Content}")) : "None recorded")}

                RECENT ENCOUNTERS ({encounters.Count}):
                {(encounters.Any() ? string.Join("\n", encounters.Select(e => $"- {e.Content}")) : "None recorded")}

                ACTIVE PRESCRIPTIONS ({prescriptions.Count}):
                {(prescriptions.Any() ? string.Join("\n", prescriptions.Select(p => $"- {p.Content}")) : "None recorded")}
                """;

            var chatService = _kernel.GetRequiredService<IChatCompletionService>();

            var history = new ChatHistory();

            history.AddSystemMessage("""
                You are a clinical assistant generating patient summaries for doctors.
                Create a clear, structured, professional summary.
                Use medical terminology appropriately but keep it readable.
                Highlight critical information like allergies and active conditions.
                Format the summary with clear sections.
                """);

            history.AddUserMessage($"""
                Generate a comprehensive clinical summary for this patient
                based on their health records:

                {context}

                Format the summary as:
                ## Patient Overview
                ## Critical Alerts (allergies, severe conditions)
                ## Active Conditions
                ## Current Medications
                ## Recent Visits
                ## Recommendations for New Doctor
                """);

            var respone = await chatService.GetChatMessageContentAsync(history, cancellationToken: cancellationToken);

            return new SummaryResponse(
                Summary: respone.Content ?? "Unable to generate summary.",
                TotalRecords: allRecords.Count(),
                HasData: true
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Summary failed for patient {PatientId}", patientId);

            return new SummaryResponse(
                Summary: "Unable to generate summary. Please try again.",
                TotalRecords: 0,
                HasData: false);
        }
    }
}
