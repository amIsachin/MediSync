using MediSync.BuildingBlocks.Domain;

namespace MediSync.Prescription.Domain.Events;

public sealed record PrescriptionSupersededEvent(
    Guid EventId,
    DateTime OccurredAt,
    Guid OldPrescriptionId,
    Guid NewPrescriptionId,
    Guid PatientId,
    Guid DoctorId
) : IDomainEvent
{
    public Guid Id => EventId;

    public PrescriptionSupersededEvent(Guid oldPrescriptionId, Guid newPrescriptionId, Guid patientId, Guid doctorId)
        : this(Guid.NewGuid(), DateTime.UtcNow, oldPrescriptionId, newPrescriptionId, patientId, doctorId)
    {
    }
}
