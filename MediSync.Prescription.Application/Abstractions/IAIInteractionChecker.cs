using MediSync.Prescription.Application.Commands.CreatePrescription;

namespace MediSync.Prescription.Application.Abstractions;

public interface IAIInteractionChecker
{
    public Task<AIInteractionResult> CheckInteractionAsync(CreatePrescriptionCommand command, CancellationToken cancellationToken = default);
}


public record AIInteractionResult(
    bool Blocked,
    string Severity,
    List<string> Interactions,
    string Recommendation
);