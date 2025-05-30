using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Heart.Commands.LoseHeart;
using Application.Features.Heart.Queries.GetUserHeart;
using Application.Features.User.Commands.Common;
using Application.Features.User.Commands.UpgradeUser;
using Application.Features.User.Commands.UserProfile.UploadProfile;
using Application.Features.User.Queries.GetAllUser;
using Application.Features.User.Queries.GetCourseUser;
using Domain.Query.User;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Contracts.Requests;
using WebApi.Extensions;
using WebApi.Infrastructure;

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

        [HttpPost("lose-heart")]
        [Authorize]
        public async Task<IActionResult> UserLoseHeart()
        {
            var command = new LoseHeartCommand();
            var result = await _mediator.Send(command);

            return result.Match(Ok, CustomResults.Problem);
        }

        [HttpGet("heart")]
        [Authorize]
        public async Task<IActionResult> GetUserCurrentHeart()
        {
            var query = new GetUserHeartQuery();
            var result = await _mediator.Send(query);

            return result.Match(Ok, CustomResults.Problem);
        }
        [HttpPost("upgrade")]
        [Authorize]
        public async Task<IActionResult> UpgradeUser([FromBody] UpgradeUserCommandDto dto)
        {
            var command = new UpgradeUserCommand(dto);
            var result = await _mediator.Send(command);

            return result.Match(Ok, CustomResults.Problem);
        }
        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers([FromQuery] GetAllUserQueryParams queryParams)
        {
            var query = new GetAllUserQuery(queryParams);
            var result = await _mediator.Send(query);

            return result.Match(Ok, CustomResults.Problem);
        }
        [HttpGet("registered-course")]
        public async Task<IActionResult> GetRegisteredCourse()
        {
            var query = new GetCourseUserQuery();
            var result = await _mediator.Send(query);

            return result.Match(Ok, CustomResults.Problem);
        }
    }
}