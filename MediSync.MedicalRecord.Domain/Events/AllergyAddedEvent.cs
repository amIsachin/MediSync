using MediSync.BuildingBlocks.Domain;

namespace MediSync.MedicalRecord.Domain.Events;

public record AllergyAddedEvent
    (
        Guid EventId,
        DateTime OccurredAt,
        Guid PatientId,
        Guid AllergyId,
        string Substance,
        string Severity
    ) : IDomainEvent
{
    public Guid Id => EventId;

    public AllergyAddedEvent(Guid patientId, Guid allergyId, string substance, string severity)
        : this(Guid.NewGuid(), DateTime.UtcNow, patientId, allergyId, substance, severity)
    { }
}
