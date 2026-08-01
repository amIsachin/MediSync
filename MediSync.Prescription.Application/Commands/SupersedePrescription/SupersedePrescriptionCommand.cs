using MediatR;
using MediSync.BuildingBlocks.Common;
using MediSync.Prescription.Domain.Enums;

namespace MediSync.Prescription.Application.Commands.SupersedePrescription;

public record SupersedePrescriptionCommand(
    Guid OldPrescriptionId,
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
