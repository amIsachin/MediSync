using MediSync.Web.IService;
using MediSync.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MediSync.Web.Controllers
{
    [Authorize]
    public class PatientController : Controller
    {
        private readonly IMedicalRecordService _medicalRecordService;

        public PatientController(IMedicalRecordService medicalRecordService)
        {
            _medicalRecordService = medicalRecordService;
        }

        [HttpGet("CreateProfile")]
        public IActionResult CreateProfile()
        {
            return View(new CreatePatientProfileViewModel());
        }

        [HttpPost("CreateProfile")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProfile(CreatePatientProfileViewModel model)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var email = User.FindFirst(ClaimTypes.Email)!.Value;

            var request = new CreatePatientProfileRequest(
                    userId,
                    model.FirstName,
                    model.LastName,
                    model.DateOfBirth,
                    model.BloodGroup,
                    model.Gender,
                    email,
                    model.PhoneNumber
                );

            var result = await _medicalRecordService.CreatePatientProfileAsync(request);

            if (!result.IsSuccess)
            {
                TempData["NotificationMessage"] = result.Error;
                TempData["NotificationType"] = "Error";
                return View(model);
            }

            TempData["NotificationMessage"] = "Medical profile created successfully.";
            TempData["NotificationType"] = "Success";
            return RedirectToAction("Index", "Home");
        }


    }
}
