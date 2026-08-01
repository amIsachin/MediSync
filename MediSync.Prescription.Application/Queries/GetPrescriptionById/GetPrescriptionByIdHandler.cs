using MediatR;
using MediSync.BuildingBlocks.Common;
using MediSync.Prescription.Application.DTOs;
using MediSync.Prescription.Domain.Errors;
using MediSync.Prescription.Domain.Interfaces;

namespace MediSync.Prescription.Application.Queries.GetPrescriptionById;

/// <summary>
/// Handles the query to retrieve a prescription by its unique identifier.
///
/// Responsibilities:
/// 1. Retrieve the prescription from the repository.
/// 2. Return a domain error if the prescription does not exist.
/// 3. Map the domain entity to a DTO.
/// 4. Return the DTO wrapped in a successful Result.
/// </summary>
public class GetPrescriptionByIdHandler : IRequestHandler<GetPrescriptionByIdQuery, Result<PrescriptionDto>>
{
    private readonly IPrescriptionRepository _prescriptionRepository;

    public GetPrescriptionByIdHandler(IPrescriptionRepository prescriptionRepository)
    {
        _prescriptionRepository = prescriptionRepository;
    }

    public async Task<Result<PrescriptionDto>> Handle(GetPrescriptionByIdQuery request, CancellationToken cancellationToken)
    {
        var prescription = await _prescriptionRepository.GetByIdAsync(request.PrescriptionId, cancellationToken);

        if (prescription is null)
        {
            return Result<PrescriptionDto>.Failure(PrescriptionErrors.NotFound(request.PrescriptionId));
        }

        var dto = GetPrescriptionByIdHandler.MapToDto(prescription);

        return Result<PrescriptionDto>.Success(dto);
    }

    /// <summary>
    /// Maps a Prescription domain entity to a PrescriptionDto.
    /// This isolates mapping logic from the query handler and keeps
    /// the Handle method focused on orchestration.
    /// </summary>
    /// <param name="prescription">The domain prescription entity.</param>
    /// <returns>A populated PrescriptionDto.</returns>
    private static PrescriptionDto MapToDto(MediSync.Prescription.Domain.Aggregates.Prescription prescription)
    {
        return new PrescriptionDto
        (
            prescription.Id,
            prescription.PatientId,
            prescription.DoctorId,
            prescription.Drug.DrugName,
            prescription.Drug.Dosage,
            prescription.Drug.GenericName,
            prescription.Dosage.Frequency.ToString(),
            prescription.Dosage.Route.ToString(),
            prescription.Dosage.DurationDays,
            prescription.Dosage.SpecialInstructions,
            prescription.Status.ToString(),
            prescription.PrescribedAt,
            prescription.ExpiresAt,
            prescription.Notes,
            prescription.SupersededById
        );
    }
}
