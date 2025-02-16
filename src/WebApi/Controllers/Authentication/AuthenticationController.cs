using Application.Features.User.Commands.Login;
using Application.Features.User.Commands.Register;
using Application.Features.User.Queries.GetMe;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using WebApi.Extensions;
using WebApi.Infrastructure;

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
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequestDto userLoginRequestDto)
        {
            var command = new UserLoginCommand(userLoginRequestDto);
            var result = await _mediator.Send(command);
            return result.Match(Ok, CustomResults.Problem);
        }
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMe()
        {
            var query = new GetMeQuery();
            var result = await _mediator.Send(query);
            return result.Match(Ok, CustomResults.Problem);
        }
    }
}
