using MediatR;
using MediSync.BuildingBlocks.Common;
using MediSync.Prescription.Application.DTOs;
using MediSync.Prescription.Domain.Interfaces;

namespace MediSync.Prescription.Application.Queries.GetPatientPrescriptions;

/// <summary>
/// Handles the query to retrieve a patient's prescriptions.
///
/// Supports retrieving:
/// - All prescriptions
/// - Only active prescriptions (based on the ActiveOnly flag)
///
/// Responsible for:
/// 1. Fetching prescription entities from the repository.
/// 2. Mapping domain entities to DTOs.
/// 3. Returning the result wrapped in a Result object.
/// </summary>
public class GetPatientPrescriptionsHandler : IRequestHandler<GetPatientPrescriptionsQuery, Result<List<PrescriptionDto>>>
{
    private readonly IPrescriptionRepository _prescriptionRepository;

    public GetPatientPrescriptionsHandler(IPrescriptionRepository prescriptionRepository)
    {
        _prescriptionRepository = prescriptionRepository;
    }

    public async Task<Result<List<PrescriptionDto>>> Handle(GetPatientPrescriptionsQuery request, CancellationToken cancellationToken)
    {
        var prescriptions = request.ActiveOnly ? await _prescriptionRepository.GetActiveByPatientIdAsync(request.PatientId, cancellationToken) : await _prescriptionRepository.GetByPatientIdAsync(request.PatientId, cancellationToken);

        var prescriptionDtos = prescriptions.Select(p => new PrescriptionDto(
              p.Id,
              p.PatientId,
              p.DoctorId,
              p.Drug.DrugName,
              p.Drug.Dosage,
              p.Drug.GenericName,
              p.Dosage.Frequency.ToString(),
              p.Dosage.Route.ToString(),
              p.Dosage.DurationDays,
              p.Dosage.SpecialInstructions,
              p.Status.ToString(),
              p.PrescribedAt,
              p.ExpiresAt,
              p.Notes,
              p.SupersededById
        )).ToList();

        return Result<List<PrescriptionDto>>.Success(prescriptionDtos);
    }
}
