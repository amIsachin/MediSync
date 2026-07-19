namespace MediSync.MedicalRecord.Domain.Enums;

/// <summary>
/// Represents the type of patient encounter with a healthcare provider.
/// Used to classify how the medical consultation or treatment was delivered.
/// </summary>
public enum EncounterType
{
    /// <summary>
    /// The patient visited the healthcare facility for a face-to-face consultation.
    /// </summary>
    InPerson = 1,

    /// <summary>
    /// The consultation was conducted remotely using telemedicine or video conferencing.
    /// </summary>
    Telemedicine = 2,

    /// <summary>
    /// The patient received immediate medical attention for an urgent or life-threatening condition.
    /// </summary>
    Emergency = 3,

    /// <summary>
    /// A scheduled follow-up visit to review treatment progress or ongoing care.
    /// </summary>
    FollowUp = 4
}