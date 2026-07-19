using MediatR;
using MediSync.BuildingBlocks.Common;
using MediSync.MedicalRecord.Application.DTOs;
using MediSync.MedicalRecord.Domain.Enums;

namespace MediSync.MedicalRecord.Application.Commands.CreatePatientProfile;

public record CreatePatientProfileCommand
    (
        Guid UserId,
        string FirstName,
        string LastName,
        DateOnly DateOfBirth,
        BloodGroup BloodGroup,
        GenderType Gender,
        string Email,
        string? PhoneNumber
    ) : IRequest<Result<Guid>>;
