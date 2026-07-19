using MediSync.BuildingBlocks.Domain;

namespace MediSync.MedicalRecord.Domain.Events;

public sealed record PatientProfileCreatedEvent
    (
        Guid EventId,
        DateTime OccurredAt,
        Guid PatientId,
        string FullName,
        string Email
    ) : IDomainEvent
{
    public Guid Id => EventId;

    public PatientProfileCreatedEvent(Guid patientId, string fullName, string email)
        : this(Guid.NewGuid(), DateTime.UtcNow, patientId, fullName, email)
    { }
}

