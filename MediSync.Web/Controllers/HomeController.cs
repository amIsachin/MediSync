using MediSync.Web.IService;
using MediSync.Web.Models;
using MediSync.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace MediSync.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IMedicalRecordService _medicalRecordService;
        private readonly IPrescriptionService _prescriptionService;

        public HomeController(IMedicalRecordService medicalRecordService, IPrescriptionService prescriptionService)
        {
            _medicalRecordService = medicalRecordService;
            _prescriptionService = prescriptionService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var role = User.FindFirst(ClaimTypes.Role)?.Value;
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                var firstName = User.FindFirst(ClaimTypes.GivenName)?.Value;
                var lastName = User.FindFirst(ClaimTypes.Surname)?.Value;
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                var token = User.FindFirst("jwt_token")?.Value;

                // Only load medical profile for Patient role
                if (role == "Patient")
                {
                    var profileResult = await _medicalRecordService.GetPatientByUserIdAsync(userId);

                    // Profile not created yet — redirect to create profile
                    if (profileResult.IsSuccess is false || profileResult.Value is null)
                    {
                        return RedirectToAction("CreateProfile", "Patient");
                    }

                    // Load active prescriptions
                    var prescriptionResult = await _prescriptionService.GetPatientPrescriptionsAsync(profileResult.Value.Id, isActiveOnly: true);

                    ViewBag.Prescriptions = prescriptionResult.IsSuccess ? prescriptionResult.Value : new List<PrescriptionResponse>();

                    return View(profileResult.Value);
                }

                if (role == "Doctor")
                {
                    var model = new DoctorDashboardViewModel
                    {
                        DoctorName = $"{firstName} {lastName}",
                        Email = email!
                    };

                    return View("DoctorDashboard", model);
                }

                return View("LabDashboard");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
