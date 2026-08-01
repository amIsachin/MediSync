using MediatR;
using MediSync.BuildingBlocks.Common;
using MediSync.Prescription.Domain.Errors;
using MediSync.Prescription.Domain.Interfaces;

namespace MediSync.Prescription.Application.Commands.CancelPrescription;

/// <summary>
/// Handles cancellation of a prescription.
/// This handler loads the prescription aggregate, invokes the domain Cancel behavior,
/// persists changes and converts domain errors into a Result response.
/// </summary>
public class CancelPrescriptionHandler : IRequestHandler<CancelPrescriptionCommand, Result<bool>>
{
    private readonly IPrescriptionRepository _prescriptionRepository;

    /// <summary>
    /// Creates a new instance of <see cref="CancelPrescriptionHandler"/>.
    /// </summary>
    /// <param name="prescriptionRepository">Repository for loading and updating prescriptions.</param>
    public CancelPrescriptionHandler(IPrescriptionRepository prescriptionRepository)
    {
        _prescriptionRepository = prescriptionRepository;
    }

    /// <summary>
    /// Handles the cancellation command.
    /// Steps:
    /// 1. Load the prescription by id.
    /// 2. If not found, return a NotFound error result.
    /// 3. Invoke the domain Cancel method to apply the cancellation logic.
    /// 4. Persist the updated aggregate and return success.
    /// </summary>
    /// <param name="request">Cancellation command containing prescription id and reason.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result indicating success (true) or failure with an error.</returns>
    public async Task<Result<bool>> Handle(CancelPrescriptionCommand request, CancellationToken cancellationToken)
    {
        // Load the prescription aggregate that will be cancelled.
        var prescription = await _prescriptionRepository.GetByIdAsync(request.PrescriptionId, cancellationToken);

        // Return a not-found result if the prescription does not exist.
        if (prescription is null)
        {
            return Result<bool>.Failure(PrescriptionErrors.NotFound(request.PrescriptionId));
        }

        // Perform the domain operation inside a try/catch to translate exceptions to a Result failure.
        try
        {
            // Apply domain behavior: mark prescription as cancelled and record reason.
            prescription.Cancel(request.Reason);
        }
        catch (Exception ex)
        {
            // Convert any domain/validation exception into a standardized failure result.
            return Result<bool>.Failure(Error.Failure("Prescription.InvalidOperation", ex.Message));
        }

        // Persist the updated aggregate state.
        await _prescriptionRepository.UpdateAsync(prescription, cancellationToken);

        // Return success indicating cancellation completed.
        return Result<bool>.Success(true);
    }
}
