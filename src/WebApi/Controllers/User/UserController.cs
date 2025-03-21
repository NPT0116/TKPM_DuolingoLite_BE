using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.User.Commands.Common;
using Application.Features.User.Commands.UserProfile.UploadProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Contracts.Requests;

namespace WebApi.Controllers.User
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;
        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("profile-image/upload")]
        [Authorize]
        public async Task<IActionResult> UploadUserProfile([FromForm] ProfileUploadRequestDto dto)
        {
            var avatar = dto.Avatar;
            AvatarUploadRequest? request = null;
            if (avatar != null)
            {
                using var memoryStream = new MemoryStream();
                await avatar.CopyToAsync(memoryStream);
                request = new AvatarUploadRequest(
                    memoryStream.ToArray(),
                    avatar.FileName,
                    avatar.ContentType
                );
            }

            var command = new UploadProfileCommand(request);
            var result = await _mediator.Send(command);
            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }
            return Ok(result);
        }
    }
}