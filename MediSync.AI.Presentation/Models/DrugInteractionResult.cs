namespace MediSync.AI.Presentation.Models;

public record DrugInteractionResult(
    bool HasInteraction,
    string Severity,
    List<string> Interactions,
    string Recommendation,
    bool IsSafeToDispense
);
