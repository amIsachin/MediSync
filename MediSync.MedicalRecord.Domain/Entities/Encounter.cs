using MediSync.BuildingBlocks.Domain;
using MediSync.MedicalRecord.Domain.Enums;

namespace MediSync.MedicalRecord.Domain.Entities;

public class Encounter : BaseEntity
{
    public Guid DoctorId { get; private set; }
    public DateTime VisitDate { get; private set; }
    public EncounterType Type { get; private set; }
    public string ChiefComplaint { get; private set; } = default!;  // why patient came
    public string? Notes { get; private set; }               // doctor's notes
    public string? Facility { get; private set; }

    private Encounter() { }

    internal static Encounter Create(Guid doctorId, EncounterType type, string chiefComplaint, string? notes = null, string? facility = null)
    {
        return new Encounter
        {
            Id = Guid.NewGuid(),
            DoctorId = doctorId,
            VisitDate = DateTime.UtcNow,
            Type = type,
            ChiefComplaint = chiefComplaint.Trim(),
            Notes = notes,
            Facility = facility
        };
    }

    internal void AddNotes(string notes)
    {
        ArgumentNullException.ThrowIfNull(notes, nameof(notes));

        Notes = notes;
    }
}
