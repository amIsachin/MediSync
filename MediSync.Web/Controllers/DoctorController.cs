using MediSync.Web.IService;
using MediSync.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MediSync.Web.Controllers
{
    public class DoctorController : Controller
    {
        private readonly IMedicalRecordService _medicalRecordService;
        private readonly IPrescriptionService _prescriptionService;

        public DoctorController(IMedicalRecordService medicalRecordService, IPrescriptionService prescriptionService)
        {
            _medicalRecordService = medicalRecordService;
            _prescriptionService = prescriptionService;
        }

        [HttpGet]
        public IActionResult FindPatient()
        {
            return View(new FindPatientViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> FindPatient(FindPatientViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _medicalRecordService.GetPatientByEmailAsync(model.Email);

            if (!result.IsSuccess || result.Value is null)
            {
                TempData["NotificationMessage"] = "Patient not found.";
                TempData["NotificationType"] = "Error";
                return View(model);
            }

            return RedirectToAction("PatientDetail", new { patientId = result.Value.Id });
        }

        [HttpGet]
        public async Task<IActionResult> PatientDetail(Guid patientId)
        {
            var profileResult = await _medicalRecordService.GetPatientByIdAsync(patientId);

            if (!profileResult.IsSuccess || profileResult.Value is null)
            {
                return RedirectToAction("FindPatient");
            }

            var prescriptionResult = await _prescriptionService.GetPatientPrescriptionsAsync(patientId);

            var model = new PatientDetailViewModel
            {
                Profile = profileResult.Value,
                Prescriptions = prescriptionResult.IsSuccess ? prescriptionResult.Value ?? new() : new List<PrescriptionResponse>()
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult WritePrescription(Guid patientId, string? patientName = null)
        {
            var model = new WritePrescriptionViewModel
            {
                PatientId = patientId,
                PatientName = patientName ?? string.Empty
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> WritePrescription(WritePrescriptionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var doctorId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            // Here you would typically call a service to save the prescription to the database.
            var command = new CreatePrescriptionRequest(
                model.PatientId,
                doctorId,
                model.DrugName,
                model.Dosage,
                model.GenericName,
                model.Frequency,
                model.Route,
                model.DurationDays,
                model.SpecialInstructions,
                model.Notes
            );

            var result = await _prescriptionService.CreatePrescriptionAsync(command);

            if (!result.IsSuccess)
            {
                TempData["NotificationMessage"] = result.Error.Message;
                TempData["NotificationType"] = "Error";
                return View(model);
            }

            TempData["NotificationMessage"] = "Prescription written successfully.";
            TempData["NotificationType"] = "Success";
            return RedirectToAction("PatientDetail", new { patientId = model.PatientId });
        }

        [HttpGet]
        public async Task<IActionResult> AddAllergy(Guid patientId, string patientName)
        {
            return View(new AddAllergyViewModel
            {
                PatientId = patientId,
                PatientName = patientName
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAllergy(AddAllergyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var doctorId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            var result = await _medicalRecordService.AddAllergyAsync(model.PatientId, new AddAllergyRequest
                 (
                     model.Substance,
                     model.Severity,
                     doctorId,
                     model.Notes
                 ));

            if (!result.IsSuccess)
            {
                TempData["NotificationMessage"] = result.Error.Message;
                TempData["NotificationType"] = "Error";
                return View(model);
            }

            TempData["NotificationMessage"] = "Allergy recorded successfully.";
            TempData["NotificationType"] = "Success";
            return RedirectToAction("PatientDetail", new { patientId = model.PatientId });
        }

        [HttpGet]
        public IActionResult AddDiagnosis(Guid patientId, string patientName)
        {
            return View(new AddDiagnosisViewModel
            {
                PatientId = patientId,
                PatientName = patientName
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddDiagnosis(AddDiagnosisViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var doctorId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var result = await _medicalRecordService.AddDiagnosisAsync(model.PatientId, new AddDiagnosisRequest
                (
                    doctorId,
                    model.IcdCode,
                    model.Description
                ));

            if (!result.IsSuccess)
            {
                TempData["NotificationMessage"] = result.Error.Message;
                TempData["NotificationType"] = "Error";
                return View(model);
            }

            TempData["NotificationMessage"] = "Diagnosis recorded successfully.";
            TempData["NotificationType"] = "Success";
            return RedirectToAction("PatientDetail", new { patientId = model.PatientId });
        }
        [HttpGet]
        public IActionResult RecordEncounter(Guid patientId, string patientName)
        {
            return View(new RecordEncounterViewModel
            {
                PatientId = patientId,
                PatientName = patientName
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecordEncounter(RecordEncounterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var doctorId = Guid.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var result = await _medicalRecordService.RecordEncounterAsync(
                model.PatientId,
                new RecordEncounterRequest(
                    doctorId,
                    model.EncounterType,
                    model.ChiefComplaint,
                    model.Notes,
                    model.Facility));

            if (!result.IsSuccess)
            {
                TempData["NotificationMessage"] = result.Error.Message;
                TempData["NotificationType"] = "Error";
                return View(model);
            }

            TempData["NotificationMessage"] = "Encounter recorded successfully.";
            TempData["NotificationType"] = "Success";
            return RedirectToAction("PatientDetail", new { patientId = model.PatientId });
        }
    }
}
