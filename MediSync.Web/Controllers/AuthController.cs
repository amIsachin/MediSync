using MediSync.Web.Services;
using MediSync.Web.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
            if (User.Identity?.IsAuthenticated is true)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new RegisterViewModel());
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

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            if (User.Identity?.IsAuthenticated is true)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            try
            {
                AuthService authService = new AuthService(_httpClient);
                var serviceResponseMessage = await authService.LoginAsync(model.Email, model.Password);

                if (serviceResponseMessage.IsSuccess)
                {
                    var claims = new List<Claim>
                    {
                        new(ClaimTypes.NameIdentifier, serviceResponseMessage.Value!.UserId.ToString()),
                        new(ClaimTypes.Email,          serviceResponseMessage.Value.Email),
                        new(ClaimTypes.GivenName,      serviceResponseMessage.Value.FirstName),
                        new(ClaimTypes.Surname,        serviceResponseMessage.Value.LastName),
                        new(ClaimTypes.Role,           serviceResponseMessage.Value.Role),
                        new("jwt_token",               serviceResponseMessage.Value.Token)
                    };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(identity),
                        new AuthenticationProperties
                        {
                            IsPersistent = model.RememberMe,
                            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
                        });

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["NotificationMessage"] = serviceResponseMessage.Error?.Message ?? "Login failed. Please try again.";
                    TempData["NotificationType"] = "Error";
                    return View(model);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }
    }
}