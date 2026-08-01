using MediSync.BuildingBlocks.Domain;

namespace MediSync.Prescription.Domain.Events;

public sealed record PrescriptionCancelledEvent(
    Guid EventId,
    DateTime OccurredAt,
    Guid PrescriptionId,
    Guid PatientId,
    string Reason
) : IDomainEvent
{
    public Guid Id => EventId;

    public PrescriptionCancelledEvent(Guid prescriptionId, Guid patientId, string reason)
        : this(Guid.NewGuid(), DateTime.UtcNow, prescriptionId, patientId, reason)
    {
    }
}
