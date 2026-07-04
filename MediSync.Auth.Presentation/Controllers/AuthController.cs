using MediatR;
using MediSync.Auth.Application.Commands.LoginUser;
using MediSync.Auth.Application.Commands.RegisterUser;
using MediSync.Auth.Domain.Utilities.Enums;
using Microsoft.AspNetCore.Mvc;

namespace MediSync.Auth.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
        {
            // Build the command from the HTTP request
            var command = new RegisterUserCommand(
                request.FirstName,
                request.LastName,
                request.Email,
                request.Password,
                request.Role);

            var result = await _mediator.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginUserCommand request, CancellationToken cancellationToken)
        {
            var command = new LoginUserCommand(request.Email, request.Password);
            var result = await _mediator.Send(command, cancellationToken);

            return Ok(result);
        }
    }

    // Request model — only used in API layer
    // Records are immutable — perfect for request/response models
    public record RegisterRequest(
        string FirstName,
        string LastName,
        string Email,
        string Password,
        UserRole Role
    );

}
