using MediSync.Web.IService;
using MediSync.Web.Models;
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

        public HomeController(IMedicalRecordService medicalRecordService)
        {
            _medicalRecordService = medicalRecordService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var role = User.FindFirst(ClaimTypes.Role)?.Value;
                var token = User.FindFirst("jwt_token")?.Value;

                // Only load medical profile for Patient role
                if (role == "Patient")
                {
                    var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                    var result = await _medicalRecordService.GetPatientByUserIdAsync(userId);

                    if (result.IsSuccess is true && result.Value is not null)
                    {
                        return View(result.Value!);
                    }

                    // Profile not created yet — redirect to create profile
                    if (result.IsSuccess is false)
                    {
                        return RedirectToAction("CreateProfile", "Patient");
                    }
                }

                //if (User.Identity?.IsAuthenticated is false)
                //{
                //    return RedirectToAction("Login", "Auth");
                //}

                return View();
            }
            catch (Exception ex)
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
