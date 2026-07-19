using MediSync.BuildingBlocks.Domain;
using MediSync.MedicalRecord.Domain.ValueObjects;

namespace MediSync.MedicalRecord.Domain.Entities;

/// <summary>
/// Represents a medical diagnosis recorded by a doctor.
/// Lives inside the Patient aggregate — cannot exist independently.
/// </summary>
public sealed class Diagnosis : BaseEntity
{
    public IcdCode IcdCode { get; private set; } = default!;  // e.g. "I10" = Hypertension
    public string Description { get; private set; } = default!;
    public Guid DoctorId { get; private set; }
    public DateTime DiagnosedAt { get; private set; }
    public bool IsActive { get; private set; }

    private Diagnosis() { }

    internal static Diagnosis Create(IcdCode icdCode, string description, Guid doctorId)
    {
        return new Diagnosis
        {
            Id = Guid.NewGuid(),
            IcdCode = icdCode,
            Description = description,
            DoctorId = doctorId,
            DiagnosedAt = DateTime.UtcNow,
            IsActive = true
        };
    }

    internal void Resolve()
    {
        IsActive = false;
    }
}
