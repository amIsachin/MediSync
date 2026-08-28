namespace MediSync.Notification.Presentation.Models;

// Sent by Prescription.API when prescription is created
public record PrescriptionCreatedNotification(
    Guid PrescriptionId,
    Guid PatientId,
    string FullName,
    string DrugName,
    string Dosage,
    string Frequency,
    string DurationDays,
    DateTime PrescribedAt,
    DateTime ExpiresAt
);

// Sent by MedicalRecord.API when patient profile is created
public record PatientProfileCreatedNotification(
    Guid PatientId,
    string PatientEmail,
    string PatientName
);

// Sent by MedicalRecord.API when diagnosis is added
public record DiagnosisAddedNotification(
    Guid PatientId,
    string PatientEmail,
    string PatientName,
    string Diagnosis,
    string IcdCode
);
