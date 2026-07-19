using MediatR;
using MediSync.BuildingBlocks.Common;
using MediSync.MedicalRecord.Application.DTOs;
using MediSync.MedicalRecord.Domain.Errors;
using MediSync.MedicalRecord.Domain.Interfaces;

namespace MediSync.MedicalRecord.Application.Queries.GetPatientByUserId;

public class GetPatientByUserIdHandler : IRequestHandler<GetPatientByUserIdQuery, Result<PatientProfileDto>>
{
    private readonly IPatientRepository _patientRepository;

    public GetPatientByUserIdHandler(IPatientRepository _patientRepository)
    {
        this._patientRepository = _patientRepository;
    }

    public async Task<Result<PatientProfileDto>> Handle(GetPatientByUserIdQuery request, CancellationToken cancellationToken)
    {
        var patient = await _patientRepository.GetByUserIdAsync(request.userId, cancellationToken);

        if (patient == null)
        {
            return Result<PatientProfileDto>.Failure(PatientErrors.NotFound(request.userId));
        }

        var dto = new PatientProfileDto
            (
                patient.Id,
                patient.UserId,
                patient.FullName.FirstName,
                patient.FullName.LastName,
                patient.FullName.ToString(),
                patient.DateOfBirth.Value,
                patient.DateOfBirth.Age,
                patient.BloodGroup.ToString(),
                patient.Gender.ToString(),
                patient.Email,
                patient.PhoneNumber,
                patient.Status.ToString(),
                patient.Allergies.Select(a => new AllergyDto(
                        a.Id,
                        a.Substance,
                        a.Severity.ToString(),
                        a.Notes,
                        a.RecordedAt
                    )).ToList(),
                patient.Diagnoses.Select(d => new DiagnosisDto(
                        d.Id,
                        d.IcdCode.ToString(),
                        d.Description,
                        d.DoctorId,
                        d.DiagnosedAt,
                        d.IsActive
                    )).ToList(),
                patient.Encounters.Select(e => new EncounterDto(
                        e.Id,
                        e.DoctorId,
                        e.Type.ToString(),
                        e.ChiefComplaint,
                        e.Notes,
                        e.Facility,
                        e.VisitDate
                    )).ToList()
            );

        return Result<PatientProfileDto>.Success(dto);
    }
}
