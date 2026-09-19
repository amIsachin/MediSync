using MediSync.Prescription.Application.Abstractions;
using MediSync.Prescription.Application.Commands.CreatePrescription;
using MediSync.Prescription.Domain.Interfaces;

namespace MediSync.Prescription.Infrastructure.Services;

public class AIInteractionChecker : IAIInteractionChecker
{
    private readonly IAIService _aiService;
    private readonly IPrescriptionRepository _prescriptionRepository;
    private readonly IPatientDataService _patientDataService;

    public AIInteractionChecker(IAIService aiService, IPrescriptionRepository prescriptionRepository, IPatientDataService patientDataService)
    {
        _aiService = aiService;
        _prescriptionRepository = prescriptionRepository;
        _patientDataService = patientDataService;
    }

    public async Task<AIInteractionResult> CheckInteractionAsync(CreatePrescriptionCommand command, CancellationToken cancellationToken = default)
    {
        // Fetch patient's active prescriptions for context
        var activePrescriptions = await _prescriptionRepository.GetActiveByPatientIdAsync(command.PatientId, cancellationToken);
        var allergies = await _patientDataService.GetPatientAllergiesAsync(command.PatientId, cancellationToken);

        var activeMeds = activePrescriptions.Select(p => new ActiveMedicationInfo(
              p.Drug.DrugName,
              p.Drug.Dosage,
              p.Dosage.Frequency.ToString()
        )).ToList();

        var allergyInfo = allergies.Select(a => new AllergyInfo(
             a.Substance,
             a.Severity
         )).ToList();

        var request = new DrugInteractionCheckRequest(
            command.PatientId,
            command.DrugName,
            command.Dosage,
            command.Frequency.ToString(),
            command.Route.ToString(),
            activeMeds,
            allergyInfo
        );

        var result = await _aiService.CheckDrugInteractionAsync(request, cancellationToken);

        return new AIInteractionResult(
            result.Blocked,
            result.Severity,
            result.Interactions,
            result.Recommendation
        );
    }
}
