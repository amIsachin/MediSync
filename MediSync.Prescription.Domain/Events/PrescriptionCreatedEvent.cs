using MediSync.BuildingBlocks.Domain;

namespace MediSync.Prescription.Domain.Events;

/// <summary>
/// Raised when a doctor writes a new prescription.
/// Listeners:
///   AI.API          → drug interaction check (synchronous via gRPC)
///   Notification.API → alert patient about new prescription
/// </summary>
public sealed record PrescriptionCreatedEvent(
    Guid EventId,
    DateTime OccurredAt,
    Guid PrescriptionId,
    Guid PatientId,
    Guid DoctorId,
    string DrugName,
    string Dosage
) : IDomainEvent
{
    public Guid Id => EventId;

    public PrescriptionCreatedEvent(Guid prescriptionId, Guid patientId, Guid doctorId, string drugName, string dosage)
        : this(Guid.NewGuid(), DateTime.UtcNow, prescriptionId, patientId, doctorId, drugName, dosage)
    {
    }
}
