using MediSync.BuildingBlocks.Domain;
using MediSync.Prescription.Domain.Enums;
using MediSync.Prescription.Domain.Events;
using MediSync.Prescription.Domain.ValueObjects;

namespace MediSync.Prescription.Domain.Aggregates;

/// <summary>
/// Prescription Aggregate Root.
///
/// Key DDD rule: Prescriptions are IMMUTABLE once created.
/// A doctor cannot edit a prescription — only supersede it.
/// Every state change raises a domain event — full audit trail.
/// </summary>
public sealed class Prescription : AggregateRoot
{
    public Guid PatientId { get; private set; }
    public Guid DoctorId { get; private set; }
    public DrugInfo Drug { get; private set; } = default!;
    public DosageInstruction Dosage { get; private set; } = default!;
    public PrescriptionStatus Status { get; private set; }
    public DateTime PrescribedAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public string? Notes { get; private set; }
    public Guid? SupersededById { get; private set; }  // new prescription ID

    private Prescription() { }

    public static Prescription Create(Guid patientId, Guid doctorId, DrugInfo drug, DosageInstruction dosage, string? notes = null)
    {
        if (patientId == Guid.Empty)
        {
            throw new ArgumentException("PatientId cannot be empty.");
        }

        if (doctorId == Guid.Empty)
        {
            throw new ArgumentException("DoctorId cannot be empty.");
        }

        var prescription = new Prescription
        {
            Id = Guid.NewGuid(),
            PatientId = patientId,
            DoctorId = doctorId,
            Drug = drug ?? throw new ArgumentNullException(nameof(drug)),
            Dosage = dosage ?? throw new ArgumentNullException(nameof(dosage)),
            Status = PrescriptionStatus.Pending,
            PrescribedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(dosage.DurationDays),
            Notes = notes
        };

        prescription.RaiseDomainEvent(new PrescriptionCreatedEvent(prescription.Id, patientId, doctorId, drug.DrugName, drug.DrugName));

        return prescription;
    }

    public void Activate()
    {
        if (Status != PrescriptionStatus.Pending)
        {
            throw new InvalidOperationException("Only pending prescriptions can be activated.");
        }

        Status = PrescriptionStatus.Active; // Assuming activation means setting to Pending
    }

    public void Cancel(string reason)
    {
        if (Status == PrescriptionStatus.Cancelled)
        {
            throw new InvalidOperationException("Already cancelled.");
        }

        if (Status == PrescriptionStatus.Superseded)
        {
            throw new InvalidOperationException("Cannot cancel a superseded prescription.");
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentException("Cancellation reason is required.");
        }

        Status = PrescriptionStatus.Cancelled;

        RaiseDomainEvent(new PrescriptionCancelledEvent(Id, PatientId, reason));
    }

    public void Supersede(Guid newPrescriptionId)
    {
        if (Status == PrescriptionStatus.Cancelled)
        {
            throw new InvalidOperationException("Cannot supersede a cancelled prescription.");
        }

        if (Status == PrescriptionStatus.Superseded)
        {
            throw new InvalidOperationException("Already superseded.");
        }

        Status = PrescriptionStatus.Superseded;
        SupersededById = newPrescriptionId;

        RaiseDomainEvent(new PrescriptionSupersededEvent(Id, newPrescriptionId, PatientId, DoctorId));
    }

    public void Expire()
    {
        if (Status != PrescriptionStatus.Active)
        {
            return; // idempotent
        }

        Status = PrescriptionStatus.Expired;
    }
}
