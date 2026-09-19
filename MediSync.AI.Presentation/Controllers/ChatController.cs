using MediSync.AI.Presentation.Models;
using MediSync.AI.Presentation.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MediSync.AI.Presentation.Controllers
{
    [Route("api/ai")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly ILogger<ChatController> _logger;

        public ChatController(IChatService charService, ILogger<ChatController> logger)
        {
            _chatService = charService;
            _logger = logger;
        }

        [HttpPost("chat")]
        public async Task<IActionResult> Chat([FromBody] ChatRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest(new { Message = "Question cannot be empty" });
            }

            var response = await _chatService.AskAsync(request, cancellationToken);

            return Ok(response);
        }
    }
}
