namespace MediSync.MedicalRecord.Domain.Enums;

/// <summary>
/// Represents the current lifecycle status of a patient within the MediSync system.
/// Used to determine whether the patient's record is available for normal operations.
/// </summary>
public enum PatientStatus
{
    Active = 1,
    Inactive = 2,
    Deactivated = 3
}
