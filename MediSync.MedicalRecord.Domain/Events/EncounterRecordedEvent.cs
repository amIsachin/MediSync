using MediSync.BuildingBlocks.Domain;

namespace MediSync.MedicalRecord.Domain.Events;

public record EncounterRecordedEvent
    (
        Guid EventId,
        DateTime OccurredAt,
        Guid PatientId,
        Guid EncounterId,
        Guid DoctorId,
        DateTime VisitDate,
        string EncounterType
    ) : IDomainEvent
{
    public Guid Id => EventId;

    public EncounterRecordedEvent(Guid patientId, Guid encounterId, Guid doctorId, DateTime visitDate, string encounterType)
       : this(Guid.NewGuid(), DateTime.UtcNow, patientId, encounterId, doctorId, visitDate, encounterType)
    { }
}
