using MediatR;
using MediSync.BuildingBlocks.Common;
using MediSync.MedicalRecord.Domain.Errors;
using MediSync.MedicalRecord.Domain.Interfaces;
using MediSync.MedicalRecord.Domain.ValueObjects;

namespace MediSync.MedicalRecord.Application.Commands.AddDiagnosis;

public class AddDiagnosisHandler : IRequestHandler<AddDiagnosisCommand, Result<Guid>>
{
    private readonly IPatientRepository _patientRepository;

    public AddDiagnosisHandler(IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository;
    }

    public async Task<Result<Guid>> Handle(AddDiagnosisCommand request, CancellationToken cancellationToken)
    {
        // Step 1 — Load Patient
        var patient = await _patientRepository.GetByIdAsync(request.PatientId, cancellationToken);

        if (patient is null)
            return Result<Guid>.Failure(PatientErrors.NotFound(request.PatientId));

        // Step 2 — Build Value Object
        IcdCode icdCode;

        try
        {
            icdCode = IcdCode.Create(request.IcdCode);
        }
        catch (ArgumentException ex)
        {
            return Result<Guid>.Failure(Error.Failure("IcdCode.Invalid", ex.Message));
        }

        // Step 3 — Call aggregate behaviour
        patient.AddDiagnosis(icdCode, request.Description, request.DoctorId);

        // Step 4 — Save changes
        await _patientRepository.UpdateAsync(patient, cancellationToken);

        return Result<Guid>.Success(patient.Id);
    }
}
