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
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            AuthService authService = new AuthService(_httpClient);
            var isSuccess = await authService.Register(model.FirstName, model.LastName, model.Email, model.Password, model.Role);

            return View();
        }
    }
}