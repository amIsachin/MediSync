using MediatR;
using MediSync.BuildingBlocks.Common;
using MediSync.Prescription.Domain.Enums;

namespace MediSync.Prescription.Application.Commands.CreatePrescription;

public record CreatePrescriptionCommand(
    Guid PatientId,
    Guid DoctorId,
    string DrugName,
    string Dosage,
    string? GenericName,
    FrequencyType Frequency,
    RouteOfAdministration Route,
    int DurationDays,
    string? SpecialInstructions,
    string? Notes
) : IRequest<Result<Guid>>;
