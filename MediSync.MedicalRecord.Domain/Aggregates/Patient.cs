using MediSync.BuildingBlocks.Domain;
using MediSync.MedicalRecord.Domain.Entities;
using MediSync.MedicalRecord.Domain.Enums;
using MediSync.MedicalRecord.Domain.Events;
using MediSync.MedicalRecord.Domain.ValueObjects;

namespace MediSync.MedicalRecord.Domain.Aggregates;

/// <summary>
/// Represents the Patient aggregate root within the Medical Record bounded context.
/// A Patient owns and controls all clinical information such as allergies,
/// diagnoses, and encounters. All modifications must go through this aggregate
/// to ensure business rules and domain invariants are enforced.
/// </summary>
public sealed class Patient : AggregateRoot
{

    // Properties
    // Represents the patient's profile information, current status,
    // and audit timestamps.
    public Guid UserId { get; private set; }  // links to Auth.API User
    public FullName FullName { get; private set; } = default!;
    public DateOfBirth DateOfBirth { get; private set; } = default!;
    public BloodGroup BloodGroup { get; private set; }
    public GenderType Gender { get; private set; }
    public string Email { get; private set; } = default!;
    public string? PhoneNumber { get; private set; }
    public PatientStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Patient"/> class.
    /// Private constructor required by EF Core.
    /// Prevents creating invalid patients outside the factory method.
    /// </summary>
    private Patient() { }

    // Child Entity Collections
    // Backing collections are private to preserve encapsulation.
    // External code can only read these collections.
    private readonly List<Allergy> _allergies = new();
    private readonly List<Diagnosis> _diagnoses = new();
    private readonly List<Encounter> _encounters = new();

    public IReadOnlyList<Allergy> Allergies => _allergies.AsReadOnly();
    public IReadOnlyList<Diagnosis> Diagnoses => _diagnoses.AsReadOnly();
    public IReadOnlyList<Encounter> Encounters => _encounters.AsReadOnly();

    /// <summary>
    /// Creates a new patient profile.
    /// Initializes the aggregate in a valid state and raises
    /// a <see cref="PatientProfileCreatedEvent"/>.
    /// </summary>
    public static Patient Create(Guid userId, FullName fullName, DateOfBirth dateOfBirth, BloodGroup bloodGroup, GenderType gender, string email, string? phoneNumber)
    {
        var patient = new Patient
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            FullName = fullName,
            DateOfBirth = dateOfBirth,
            BloodGroup = bloodGroup,
            Gender = gender,
            Email = email,
            PhoneNumber = phoneNumber,
            Status = PatientStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        patient.RaiseDomainEvent(new PatientProfileCreatedEvent(patient.Id, patient.FullName.DisplayName, patient.Email));
        return patient;
    }

    public void AddAllergy(string substance, AllergySeverity severity, Guid doctorId, string? notes)
    {
        EnsurePatientIsActive();

        if (_allergies.Any(a => a.Substance.Equals(substance, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException($"Allergy to '{substance}' is already recorded.");
        }

        var allergy = Allergy.Create(substance, severity, doctorId, notes);

        _allergies.Add(allergy);

        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new AllergyAddedEvent(Id, allergy.Id, substance, severity.ToString()));
    }

    public void AddDiagnosis(IcdCode icdCode, string description, Guid doctorId)
    {
        EnsurePatientIsActive();

        var diagnosis = Diagnosis.Create(icdCode, description, doctorId);

        _diagnoses.Add(diagnosis);
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new DiagnosisAddedEvent(Id, diagnosis.Id, diagnosis.DoctorId, icdCode.Value, diagnosis.Description));
    }

    public void RecordEncounter(Guid doctorId, EncounterType type, string chiefComplaint, string? notes = null, string? facility = null)
    {
        EnsurePatientIsActive();

        var encounter = Encounter.Create(doctorId, type, chiefComplaint, notes, facility);
        _encounters.Add(encounter);
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new EncounterRecordedEvent(Id, encounter.Id, doctorId, encounter.VisitDate, encounter.Type.ToString()));
    }

    public void ResolveDiagnosis(Guid diagnosisId)
    {
        EnsurePatientIsActive();

        var diagnosis = _diagnoses.FirstOrDefault(d => d.Id == diagnosisId)
            ?? throw new InvalidOperationException($"Diagnosis {diagnosisId} not found for this patient.");

        diagnosis.Resolve();
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        if (Status == PatientStatus.Deactivated)
            return;  // idempotent — already deactivated

        Status = PatientStatus.Deactivated;
        UpdatedAt = DateTime.UtcNow;
    }

    // Reusable validation — called before every behaviour
    private void EnsurePatientIsActive()
    {
        if (Status == PatientStatus.Deactivated)
        {
            throw new InvalidOperationException("Cannot modify a deactivated patient record.");
        }
    }
}
