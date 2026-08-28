using MediSync.Notification.Presentation.Models;
using MediSync.Notification.Presentation.Services;
using Microsoft.AspNetCore.Mvc;

namespace MediSync.Notification.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<NotificationController> _logger;
        private readonly IPatientService _patientService;

        public NotificationController(IEmailService emailService, ILogger<NotificationController> logger, IPatientService phoneService)
        {
            _emailService = emailService;
            _logger = logger;
            _patientService = phoneService;
        }

        [HttpPost("prescription-created")]
        public async Task<IActionResult> PrescriptionCreated([FromBody] PrescriptionCreatedNotification request, CancellationToken cancellationToken)
        {
            // Fetch patient contact details from MedicalRecord.API
            var patient = await _patientService.GetPatientContactAsync(request.PatientId, cancellationToken);

            _logger.LogInformation("Sending prescription notification to {Email}", patient!.Value!.Email);

            if (patient.IsFailure)
            {
                _logger.LogWarning("Patient {PatientId} not found — skipping notification", request.PatientId);
                return Ok(new { Message = "Patient not found — notification skipped" });
            }

            var email = new EmailMessage(patient.Value.Email, patient.Value.FullName, "New Prescription — MediSync", NotificationController.BuildPrescriptionEmail(request));

            await _emailService.SendAsync(email, cancellationToken);

            return Ok(new { Message = "Notification sent" });
        }

        [HttpPost("profile-created")]
        public async Task<IActionResult> ProfileCreated([FromBody] PatientProfileCreatedNotification request, CancellationToken cancellationToken)
        {
            var email = new EmailMessage(request.PatientEmail, request.PatientName, "Welcome to MediSync", BuildWelcomeEmail(request));

            await _emailService.SendAsync(email, cancellationToken);

            return Ok(new { Message = "Welcome notification sent" });
        }

        [HttpPost("diagnosis-added")]
        public async Task<IActionResult> DiagnosisAdded([FromBody] DiagnosisAddedNotification request, CancellationToken cancellationToken)
        {
            var email = new EmailMessage(request.PatientEmail, request.PatientName, "New Diagnosis Recorded — MediSync", BuildDiagnosisEmail(request));

            await _emailService.SendAsync(email, cancellationToken);

            return Ok(new { Message = "Diagnosis notification sent" });
        }


        // ── Email Templates ───────────────────────────────────
        private static string BuildPrescriptionEmail(PrescriptionCreatedNotification r) => $"""
        <div style="font-family:'Segoe UI',sans-serif;max-width:560px;margin:0 auto;">
            <div style="background:#1B4F8A;padding:24px 32px;border-radius:8px 8px 0 0;">
                <h1 style="color:#fff;margin:0;font-size:20px;">🏥 MediSync</h1>
                <p style="color:rgba(255,255,255,0.7);margin:4px 0 0;font-size:13px;">
                    Connecting Health Solutions
                </p>
            </div>
            <div style="background:#fff;padding:32px;border:1px solid #e2e8f0;
                        border-top:none;border-radius:0 0 8px 8px;">
                <h2 style="color:#1a202c;font-size:18px;margin:0 0 8px;">
                    New Prescription Written
                </h2>
                <p style="color:#718096;font-size:14px;margin:0 0 24px;">
                    Dear {r.FullName}, your doctor has written a new prescription.
                </p>
                <div style="background:#f7fafc;border:1px solid #e2e8f0;
                            border-radius:8px;padding:20px;margin-bottom:24px;">
                    <table style="width:100%;border-collapse:collapse;">
                        <tr>
                            <td style="padding:6px 0;color:#718096;font-size:13px;width:140px;">
                                Drug
                            </td>
                            <td style="padding:6px 0;color:#1a202c;
                                       font-size:13px;font-weight:600;">
                                {r.DrugName} {r.Dosage}
                            </td>
                        </tr>
                        <tr>
                            <td style="padding:6px 0;color:#718096;font-size:13px;">
                                Frequency
                            </td>
                            <td style="padding:6px 0;color:#1a202c;font-size:13px;">
                                {r.Frequency}
                            </td>
                        </tr>
                        <tr>
                            <td style="padding:6px 0;color:#718096;font-size:13px;">
                                Duration
                            </td>
                            <td style="padding:6px 0;color:#1a202c;font-size:13px;">
                                {r.DurationDays} days
                            </td>
                        </tr>
                        <tr>
                            <td style="padding:6px 0;color:#718096;font-size:13px;">
                                Expires
                            </td>
                            <td style="padding:6px 0;color:#1a202c;font-size:13px;">
                                {r.ExpiresAt:dd MMM yyyy}
                            </td>
                        </tr>
                    </table>
                </div>
                <p style="color:#718096;font-size:12px;margin:0;">
                    This is an automated notification from MediSync.
                    Please do not reply to this email.
                </p>
            </div>
        </div>
        """;

        private static string BuildWelcomeEmail(
            PatientProfileCreatedNotification r) => $"""
        <div style="font-family:'Segoe UI',sans-serif;max-width:560px;margin:0 auto;">
            <div style="background:#1B4F8A;padding:24px 32px;border-radius:8px 8px 0 0;">
                <h1 style="color:#fff;margin:0;font-size:20px;">🏥 MediSync</h1>
                <p style="color:rgba(255,255,255,0.7);margin:4px 0 0;font-size:13px;">
                    Connecting Health Solutions
                </p>
            </div>
            <div style="background:#fff;padding:32px;border:1px solid #e2e8f0;
                        border-top:none;border-radius:0 0 8px 8px;">
                <h2 style="color:#1a202c;font-size:18px;margin:0 0 8px;">
                    Welcome to MediSync, {r.PatientName}!
                </h2>
                <p style="color:#718096;font-size:14px;margin:0 0 16px;">
                    Your medical profile has been created successfully.
                    You can now view your health records, prescriptions,
                    and appointments all in one place.
                </p>
                <p style="color:#718096;font-size:12px;margin:0;">
                    This is an automated notification from MediSync.
                </p>
            </div>
        </div>
        """;

        private static string BuildDiagnosisEmail(
            DiagnosisAddedNotification r) => $"""
        <div style="font-family:'Segoe UI',sans-serif;max-width:560px;margin:0 auto;">
            <div style="background:#1B4F8A;padding:24px 32px;border-radius:8px 8px 0 0;">
                <h1 style="color:#fff;margin:0;font-size:20px;">🏥 MediSync</h1>
            </div>
            <div style="background:#fff;padding:32px;border:1px solid #e2e8f0;
                        border-top:none;border-radius:0 0 8px 8px;">
                <h2 style="color:#1a202c;font-size:18px;margin:0 0 8px;">
                    New Diagnosis Recorded
                </h2>
                <p style="color:#718096;font-size:14px;margin:0 0 16px;">
                    Dear {r.PatientName}, your doctor has recorded a new diagnosis.
                </p>
                <div style="background:#f7fafc;border:1px solid #e2e8f0;
                            border-radius:8px;padding:20px;margin-bottom:24px;">
                    <p style="margin:0;color:#1a202c;font-size:14px;font-weight:600;">
                        {r.Diagnosis}
                    </p>
                    <p style="margin:4px 0 0;color:#718096;font-size:12px;">
                        ICD Code: {r.IcdCode}
                    </p>
                </div>
                <p style="color:#718096;font-size:12px;margin:0;">
                    This is an automated notification from MediSync.
                </p>
            </div>
        </div>
        """;
    }
}