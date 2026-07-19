namespace MediSync.MedicalRecord.Domain.Enums;

/// <summary>
/// Represents the severity level of an allergy.
/// This enum is used to classify how serious an allergic reaction may be,
/// helping healthcare providers prioritize treatment and clinical decisions.
/// </summary>
public enum AllergySeverity
{
    /// <summary>
    /// Mild allergic reaction with minor symptoms that usually do not require emergency treatment.
    /// </summary>
    Mild = 1,

    /// <summary>
    /// Moderate allergic reaction that may require medical attention but is not immediately life-threatening.
    /// </summary>
    Moderate = 2,

    /// <summary>
    /// Severe allergic reaction that can be life-threatening and requires immediate medical intervention.
    /// </summary>
    Severe = 3
}