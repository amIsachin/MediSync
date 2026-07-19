using MediatR;
using MediSync.BuildingBlocks.Common;
using MediSync.MedicalRecord.Domain.Aggregates;
using MediSync.MedicalRecord.Domain.Errors;
using MediSync.MedicalRecord.Domain.Interfaces;
using MediSync.MedicalRecord.Domain.ValueObjects;

namespace MediSync.MedicalRecord.Application.Commands.CreatePatientProfile;

public class CreatePatientProfileHandler : IRequestHandler<CreatePatientProfileCommand, Result<Guid>>
{
    private readonly IPatientRepository _patientRepository;

    public CreatePatientProfileHandler(IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository;
    }

    public async Task<Result<Guid>> Handle(CreatePatientProfileCommand request, CancellationToken cancellationToken)
    {
        var exists = await _patientRepository.ExistsByUserIdAsync(request.UserId, cancellationToken);

        if (exists)
        {
            return Result<Guid>.Failure(PatientErrors.AlreadyExists(request.UserId));
        }

        // Step 2 — Build Value Objects
        var fullName = FullName.Create(request.FirstName, request.LastName);
        var dateOfBirth = DateOfBirth.From(request.DateOfBirth);

        var patient = Patient.Create(request.UserId, fullName, dateOfBirth, request.BloodGroup, request.Gender, request.Email, request.PhoneNumber);

        // Step 4 — Persist
        await _patientRepository.AddAsync(patient, cancellationToken);

        // Step 5 — Return patient ID
        return Result<Guid>.Success(patient.Id);
    }
}