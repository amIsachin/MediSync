using MediSync.BuildingBlocks.Domain;

namespace MediSync.MedicalRecord.Domain.Events;

public record DiagnosisAddedEvent
    (
        Guid EventId,
        DateTime OccurredAt,
        Guid PatientId,
        Guid DiagnosisId,
        Guid DoctorId,
        string IcdCode,
        string Description
    ) : IDomainEvent
{
    public Guid Id => EventId;

    public DiagnosisAddedEvent(Guid patientId, Guid diagnosisId, Guid doctorId, string icdCode, string description)
        : this(Guid.NewGuid(), DateTime.UtcNow, patientId, diagnosisId, doctorId, icdCode, description)
    { }
}
