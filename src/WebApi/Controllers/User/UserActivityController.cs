using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Interface;
using SharedKernel;
using Application.Common.Interface;
using Application.Features.User.Queries.GetUserActivityRange;
using MediatR;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserActivityController : ControllerBase
    {
        private readonly IStreakService _streakService;
        private readonly IIdentityService _identityService;
        private readonly IMediator _mediator;

        public UserActivityController(IStreakService streakService, IIdentityService identityService, IMediator mediator)
        {
            _streakService = streakService;
            _identityService = identityService;
            _mediator = mediator;
        }

        [HttpPost("record-activity")]
        public async Task<ActionResult<bool>> RecordActivity()
        {
            var user = await _identityService.GetCurrentUserAsync();
            if (user == null)
            {
                return Unauthorized();
            }

            var result = await _streakService.RecordActivityAsync(user.Id);
            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }
        [HttpGet("get-user-activity-range")]
        public async Task<ActionResult<int>> GetStreak([FromQuery]GetUserAcitivityRangeQueryParam getUserAcitivityRangeQueryParam)
        {
            var activities = await _mediator.Send(new GetUserAcitivityRangeQuery(getUserAcitivityRangeQueryParam));
            return Ok(activities);
        }

    }
} 