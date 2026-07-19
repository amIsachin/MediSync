namespace MediSync.MedicalRecord.Domain.Enums;

/// <summary>
/// Represents the standard human blood groups using the ABO and Rh blood group systems.
/// This enum is used throughout the medical record domain to ensure type safety,
/// consistency, and validation when storing or processing a patient's blood group.
/// </summary>
public enum BloodGroup
{
    APositive = 1,      // A+
    ANegative = 2,      // A-
    BPositive = 3,      // B+
    BNegative = 4,      // B-
    ABPositive = 5,     // AB+
    ABNegative = 6,     // AB-
    OPositive = 7,      // O+
    ONegative = 8       // O-
}