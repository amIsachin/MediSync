using MediSync.Web.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MediSync.Web.Controllers
{
    
    public class ChatController : Controller
    {
        private readonly IAIChatService _aiChatService;

        public ChatController(IAIChatService aiChatService)
        {
            _aiChatService = aiChatService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index([FromBody] ChatAskRequest request)
        {
            if (string.IsNullOrEmpty(request.Message))
            {
                return BadRequest(new { error = "Message cannot be empty" });
            }

            var patientId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var firstName = User.FindFirst(ClaimTypes.GivenName)?.Value ?? "";
            var lastName = User.FindFirst(ClaimTypes.Surname)?.Value ?? "";
            var patientName = $"{firstName} {lastName}".Trim();

            var response = await _aiChatService.AskAsync(patientId, patientName, request.Message);

            return Ok(new
            {
                answer = response.Answer,
                hasRelevantData = response.HasRelevantData
            });
        }
    }
}


public record ChatAskRequest(string Message);