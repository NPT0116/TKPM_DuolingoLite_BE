using Application.Features.Media.Commands.Upload;
using Application.Features.User.Commands.Common;
using Application.Features.User.Commands.Login;
using Application.Features.User.Commands.Register;
using Application.Features.User.Queries.GetMe;
using Application.Features.User.Queries.GetUserProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using WebApi.Contracts.Requests;
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
        public async Task<IActionResult> Register(
            [FromForm] UserRegisterRequestDto userRegisterRequestDto
        )
        {
            AvatarUploadRequest? request = null;
            if (userRegisterRequestDto.Avatar != null)
            {
                using var memoryStream = new MemoryStream();
                await userRegisterRequestDto.Avatar.CopyToAsync(memoryStream);
                request = new AvatarUploadRequest(
                    memoryStream.ToArray(),
                    userRegisterRequestDto.Avatar.FileName,
                    userRegisterRequestDto.Avatar.ContentType
                );
            }
            var command = new UserRegisterCommand(
                userRegisterRequestDto.UserRegisterDto,
                request
            );
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
        // [Authorize]
        public async Task<IActionResult> GetMe()
        {
            var query = new GetUserProfileQuery();
            var result = await _mediator.Send(query);
            return result.Match(Ok, CustomResults.Problem);
        }

    }
}
