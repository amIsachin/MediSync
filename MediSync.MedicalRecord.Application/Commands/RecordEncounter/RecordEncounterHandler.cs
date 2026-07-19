using MediatR;
using MediSync.BuildingBlocks.Common;
using MediSync.MedicalRecord.Domain.Errors;
using MediSync.MedicalRecord.Domain.Interfaces;

namespace MediSync.MedicalRecord.Application.Commands.RecordEncounter;

public class RecordEncounterHandler : IRequestHandler<RecordEncounterCommand, Result<Guid>>
{
    private readonly IPatientRepository _patientRepository;

    public RecordEncounterHandler(IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository;
    }

    public async Task<Result<Guid>> Handle(RecordEncounterCommand request, CancellationToken cancellationToken)
    {
        // Step 1 — Load Patient
        var patient = await _patientRepository.GetByIdAsync(request.PatientId, cancellationToken);

        if (patient is null)
        {
            return Result<Guid>.Failure(PatientErrors.NotFound(request.PatientId));
        }

        // Step 2 — Call aggregate behaviour
        patient.RecordEncounter(request.DoctorId, request.EncounterType, request.ChiefComplaint, request.Notes, request.Facility);

        // Step 3 — Save changes
        await _patientRepository.UpdateAsync(patient, cancellationToken);

        return Result<Guid>.Success(patient.Id);
    }
}
