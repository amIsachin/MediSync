using MediatR;
using MediSync.BuildingBlocks.Common;
using MediSync.Prescription.Domain.Errors;
using MediSync.Prescription.Domain.Interfaces;
using MediSync.Prescription.Domain.ValueObjects;

namespace MediSync.Prescription.Application.Commands.SupersedePrescription;

/// <summary>
/// Handles the SupersedePrescriptionCommand by creating a new prescription and marking
/// the previous prescription as superseded.
/// </summary>
public class SupersedePrescriptionHandler : IRequestHandler<SupersedePrescriptionCommand, Result<Guid>>
{
    private readonly IPrescriptionRepository _prescriptionRepository;

    /// <summary>
    /// Initializes a new instance of <see cref="SupersedePrescriptionHandler"/>.
    /// </summary>
    /// <param name="prescriptionRepository">Repository used to load and persist prescriptions.</param>
    public SupersedePrescriptionHandler(IPrescriptionRepository prescriptionRepository)
    {
        _prescriptionRepository = prescriptionRepository;
    }

    /// <summary>
    /// Processes the command to supersede an existing prescription.
    /// Steps performed:
    /// 1. Load the existing (old) prescription.
    /// 2. Validate existence of the old prescription.
    /// 3. Build a new prescription aggregate from the command data and persist it.
    /// 4. Mark the old prescription as superseded using the domain behavior.
    /// </summary>
    /// <param name="request">The command request containing data for the new prescription and the id of the old one.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result containing the Id of the newly created prescription on success, or a failure result.</returns>
    public async Task<Result<Guid>> Handle(SupersedePrescriptionCommand request, CancellationToken cancellationToken)
    {
        // Load the existing prescription that will be superseded.
        var oldPrescription = await _prescriptionRepository.GetByIdAsync(request.OldPrescriptionId, cancellationToken);

        // If the old prescription does not exist, return a NotFound error.
        if (oldPrescription is null)
        {
            return Result<Guid>.Failure(PrescriptionErrors.NotFound(request.OldPrescriptionId));
        }

        // Build value objects for the new prescription from the incoming command.
        DrugInfo drugInfo = DrugInfo.Create(request.DrugName, request.Dosage, request.GenericName);
        DosageInstruction dosageInstruction = DosageInstruction.Create(request.Frequency, request.Route, request.DurationDays, request.SpecialInstructions);

        // Create the new prescription aggregate.
        var prescription = MediSync.Prescription.Domain.Aggregates.Prescription.Create(request.PatientId, request.DoctorId, drugInfo, dosageInstruction, request.Notes);

        // Persist the new prescription.
        await _prescriptionRepository.AddAsync(prescription);

        // Attempt to supersede the old prescription. Wrap domain operation in try/catch to
        // convert unexpected exceptions into a consistent Result failure.
        try
        {
            // The domain method Supersede is expected to perform state transition and
            // possibly produce domain events. Here we mark the old prescription as superseded.
            prescription.Supersede(prescription.Id);
        }
        catch (Exception ex)
        {
            // If the domain operation fails, return a failure result with an appropriate error code/message.
            return Result<Guid>.Failure(Error.Failure("Prescription.InvalidOperation", ex.Message));
        }

        // Return success with the new prescription Id.
        return Result<Guid>.Success(prescription.Id);
    }
}
