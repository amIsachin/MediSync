namespace MediSync.Auth.Domain.Constants;

/// <summary>
/// Defines the custom claim types used throughout the MediSync application.
/// These claims are included in JWT tokens and used for authentication
/// and authorization across services.
/// </summary>
public static class MediSyncClaimTypes
{
    /// <summary>
    /// Identifies the user's domain identifier.
    /// </summary>
    public const string UserId = "medisync:user_id";

    /// <summary>
    /// Identifies the user's role.
    /// </summary>
    public const string Role = "medisync:role";

    /// <summary>
    /// Identifies the patient's identifier.
    /// Included only for users with the Patient role.
    /// </summary>
    public const string PatientId = "medisync:patient_id";

    /// <summary>
    /// Identifies the doctor's identifier.
    /// Included only for users with the Doctor role.
    /// </summary>
    public const string DoctorId = "medisync:doctor_id";

    /// <summary>
    /// Identifies the doctor's specialty.
    /// Included only for users with the Doctor role.
    /// </summary>
    public const string Specialty = "medisync:specialty";

    /// <summary>
    /// Identifies the user's current account status.
    /// </summary>
    public const string Status = "medisync:status";
}
