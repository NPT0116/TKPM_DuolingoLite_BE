using Application.Features.User.Commands.Register;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.Authentication
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthenticationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto userRegisterDto)
        {
            var command = new UserRegisterCommand(userRegisterDto);
            var result = await _mediator.Send(command);
            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }
            return Ok(result);
        }
    }
}
