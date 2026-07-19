using MediSync.BuildingBlocks.Domain;
using MediSync.MedicalRecord.Domain.Enums;

namespace MediSync.MedicalRecord.Domain.Entities;

/// <summary>
/// Represents a known allergy for a patient.
/// Lives inside the Patient aggregate — cannot exist independently.
/// </summary>
public sealed class Allergy : BaseEntity
{
    public string Substance { get; private set; } = default!;
    public AllergySeverity Severity { get; set; }
    public string Notes { get; set; } = default!;
    public DateTime RecordedAt { get; private set; }
    public Guid RecordedBy { get; private set; }

    private Allergy() { }

    internal static Allergy Create(string substance, AllergySeverity severity, Guid recordedBy, string? notes = null)
    {
        return new Allergy
        {
            Id = Guid.NewGuid(),
            Substance = substance,
            Severity = severity,
            Notes = notes!,
            RecordedAt = DateTime.UtcNow,
            RecordedBy = recordedBy
        };
    }
}
