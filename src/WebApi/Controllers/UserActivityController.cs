using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Interface;
using SharedKernel;
using Application.Common.Interface;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserActivityController : ControllerBase
    {
        private readonly IStreakService _streakService;
        private readonly IIdentityService _identityService;

        public UserActivityController(IStreakService streakService, IIdentityService identityService)
        {
            _streakService = streakService;
            _identityService = identityService;
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

        [HttpGet("streak-status")]
        public async Task<ActionResult<(int currentStreak, int longestStreak, bool hasActivityToday)>> GetStreakStatus()
        {
            var user = await _identityService.GetCurrentUserAsync();
            if (user == null)
            {
                return Unauthorized();
            }

            var result = await _streakService.GetStreakStatusAsync(user.Id);
            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }
    }
} 