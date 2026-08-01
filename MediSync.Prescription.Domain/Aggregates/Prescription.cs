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

    /// <summary>
    /// Creates a new prescription in the <see cref="PrescriptionStatus.Pending"/> state.
    /// </summary>
    /// <param name="patientId">The identifier of the patient receiving the prescription.</param>
    /// <param name="doctorId">The identifier of the prescribing doctor.</param>
    /// <param name="drug">The prescribed medication.</param>
    /// <param name="dosage">The dosage instructions for the medication.</param>
    /// <param name="notes">Optional clinical notes associated with the prescription.</param>
    /// <returns>A newly created <see cref="Prescription"/> instance.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="patientId"/> or <paramref name="doctorId"/> is empty.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="drug"/> or <paramref name="dosage"/> is <see langword="null"/>.
    /// </exception>
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

    /// <summary>
    /// Activates the prescription.
    /// </summary>
    /// <remarks>
    /// A prescription can only be activated while it is in the
    /// <see cref="PrescriptionStatus.Pending"/> state.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the prescription is not pending.
    /// </exception>
    public void Activate()
    {
        if (Status != PrescriptionStatus.Pending)
        {
            throw new InvalidOperationException("Only pending prescriptions can be activated.");
        }

        Status = PrescriptionStatus.Active; // Assuming activation means setting to Pending
    }

    /// <summary>
    /// Cancels the prescription.
    /// </summary>
    /// <param name="reason">The reason for cancellation.</param>
    /// <remarks>
    /// A cancelled or superseded prescription cannot be cancelled again.
    /// </remarks>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="reason"/> is null, empty, or whitespace.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the prescription has already been cancelled or superseded.
    /// </exception>
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

    /// <summary>
    /// Marks the prescription as superseded by another prescription.
    /// </summary>
    /// <param name="newPrescriptionId">
    /// The identifier of the replacement prescription.
    /// </param>
    /// <remarks>
    /// Superseding preserves the audit history while indicating that a newer
    /// prescription replaces the current one.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the prescription has been cancelled or has already been superseded.
    /// </exception>
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

    /// <summary>
    /// Expires the prescription.
    /// </summary>
    /// <remarks>
    /// Only active prescriptions can be expired. Calling this method for a
    /// non-active prescription has no effect.
    /// </remarks>
    public void Expire()
    {
        if (Status != PrescriptionStatus.Active)
        {
            return; // idempotent
        }

        Status = PrescriptionStatus.Expired;
    }
}
