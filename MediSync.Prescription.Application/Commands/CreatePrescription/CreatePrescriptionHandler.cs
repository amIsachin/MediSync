using MediatR;
using MediSync.BuildingBlocks.Common;
using MediSync.Prescription.Application.Abstractions;
using MediSync.Prescription.Domain.Errors;
using MediSync.Prescription.Domain.Interfaces;
using MediSync.Prescription.Domain.ValueObjects;

namespace MediSync.Prescription.Application.Commands.CreatePrescription;

public sealed class CreatePrescriptionHandler : IRequestHandler<CreatePrescriptionCommand, Result<Guid>>
{
    private readonly IPrescriptionRepository _prescriptionRepository;
    private readonly IAIInteractionChecker _aiInteractionChecker;
    private readonly IAIPrescriptionIndexService _aiPrescriptionIndexService;

    public CreatePrescriptionHandler(IPrescriptionRepository prescriptionRepository, IAIInteractionChecker aiInteractionChecker, IAIPrescriptionIndexService aiPrescriptionIndexService)
    {
        _prescriptionRepository = prescriptionRepository;
        _aiInteractionChecker = aiInteractionChecker;
        _aiPrescriptionIndexService = aiPrescriptionIndexService;
    }

    public async Task<Result<Guid>> Handle(CreatePrescriptionCommand request, CancellationToken cancellationToken)
    {
        var drug = DrugInfo.Create(request.DrugName, request.Dosage, request.GenericName);
        var dosage = DosageInstruction.Create(request.Frequency, request.Route, request.DurationDays, request.SpecialInstructions);

        // Check drug interaction BEFORE saving
        var interactionResult = await _aiInteractionChecker.CheckInteractionAsync(request, cancellationToken);

        if (interactionResult.Blocked)
        {
            return Result<Guid>.Failure(PrescriptionErrors.DrugInteraction(interactionResult.Interactions, interactionResult.Recommendation));
        }

        // Safe to proceed — create aggregate
        var prescription = MediSync.Prescription.Domain.Aggregates.Prescription.Create(request.PatientId, request.DoctorId, drug, dosage, request.Notes);

        // Activate immediately (passed AI check)
        prescription.Activate();

        await _prescriptionRepository.AddAsync(prescription);

        _ = Task.Run(() => _aiPrescriptionIndexService.IndexPrescriptionsAsync(request.PatientId, CancellationToken.None));

        return Result<Guid>.Success(prescription.Id);
    }
}
