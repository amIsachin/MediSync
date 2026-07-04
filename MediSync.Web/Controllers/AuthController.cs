using MediSync.Web.Services;
using MediSync.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace MediSync.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IHttpClientFactory _httpClient;

        public AuthController(IHttpClientFactory httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            AuthService authService = new AuthService(_httpClient);
            var response = await authService.RegisterAsync(model.FirstName, model.LastName, model.Email, model.Password, model.Role);

            if (response.IsSuccess)
            {
                TempData["NotificationMessage"] = "Your account has been created. You can now sign in to MediSync.";
                TempData["NotificationType"] = "Success";

                return View(model);
            }

            TempData["NotificationType"] = "Failure";
            TempData["NotificationMessage"] = response.Error?.Message ?? "Registration failed. Please try again.";

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {

            try
            {
                AuthService authService = new AuthService(_httpClient);
                var isSuccess = await authService.LoginAsync(model.Email, model.Password);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}