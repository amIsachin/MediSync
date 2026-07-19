using MediatR;
using MediSync.BuildingBlocks.Common;
using MediSync.MedicalRecord.Domain.Errors;
using MediSync.MedicalRecord.Domain.Interfaces;

namespace MediSync.MedicalRecord.Application.Commands.AddAllergy;

public class AddAllergyHandler : IRequestHandler<AddAllergyCommand, Result<Guid>>
{
    private readonly IPatientRepository _patientRepository;

    public AddAllergyHandler(IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository;
    }

    public async Task<Result<Guid>> Handle(AddAllergyCommand request, CancellationToken cancellationToken)
    {
        // Step 1 — Load Patient aggregate
        var patient = await _patientRepository.GetByIdAsync(request.PatientId, cancellationToken);

        if (patient is null)
            return Result<Guid>.Failure(PatientErrors.NotFound(request.PatientId));

        // Step 2 — Call aggregate behaviour
        // Business rules enforced inside Patient.AddAllergy()
        try
        {
            patient.AddAllergy(request.Substance, request.Severity, request.DoctorId, request.Notes);
        }
        catch (InvalidOperationException ex)
        {
            return Result<Guid>.Failure(Error.Conflict("Patient.AllergyExists", ex.Message));
        }

        // Step 3 — Save changes
        await _patientRepository.UpdateAsync(patient, cancellationToken);

        return Result<Guid>.Success(patient.Id);
    }
}
