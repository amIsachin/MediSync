using MediatR;
using MediSync.BuildingBlocks.Common;
using MediSync.MedicalRecord.Application.DTOs;
using MediSync.MedicalRecord.Domain.Entities;
using MediSync.MedicalRecord.Domain.Errors;
using MediSync.MedicalRecord.Domain.Interfaces;

namespace MediSync.MedicalRecord.Application.Queries.GetPatientProfile;

public class GetPatientProfileHandler : IRequestHandler<GetPatientProfileQuery, Result<PatientProfileDto>>
{
    private readonly IPatientRepository _patientRepository;

    public GetPatientProfileHandler(IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository;
    }

    public async Task<Result<PatientProfileDto>> Handle(GetPatientProfileQuery request, CancellationToken cancellationToken)
    {
        // Step 1 — Load Patient
        var patient = await _patientRepository.GetByIdAsync(request.patientId, cancellationToken);

        if (patient is null)
        {
            return Result<PatientProfileDto>.Failure(PatientErrors.NotFound(request.patientId));
        }

        // Step 2 — Map to DTO
        // Never return the domain aggregate directly
        var dto = new PatientProfileDto
            (
                patient.Id,
                patient.UserId,
                patient.FullName.FirstName,
                patient.FullName.LastName,
                patient.FullName.DisplayName,
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
                    a.RecordedAt)).ToList(),
                patient.Diagnoses.Select(d=>new DiagnosisDto(
                    d.Id,
                    d.IcdCode.Value,
                    d.Description,
                    d.DoctorId,
                    d.DiagnosedAt,
                    d.IsActive)).ToList(),
                patient.Encounters.Select(e => new EncounterDto(
                    e.Id,
                    e.DoctorId,
                    e.Type.ToString(),
                    e.ChiefComplaint,
                    e.Notes,
                    e.Facility,
                    e.VisitDate)).ToList()
            );

        return Result<PatientProfileDto>.Success(dto);
    }
}
