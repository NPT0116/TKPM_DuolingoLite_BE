using System;
using System.Threading.Tasks;
using Application.Features.Learning.SpacedRepetition.Commands.ProcessLessonReport;
using Application.Features.Learning.SpacedRepetition.Commands.UpdateReview;
using Application.Features.Learning.SpacedRepetition.Common;
using Application.Features.Learning.SpacedRepetition.Queries.GetDueReviews;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Extensions;
using WebApi.Infrastructure;

namespace WebApi.Controllers.Learning
{
    [ApiController]
    [Route("api/spaced-repetition")]
    [Authorize]
    public class SpacedRepetitionController : ControllerBase
    {
        private readonly ISender _mediator;

        public SpacedRepetitionController(ISender mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("lesson-report")]
        public async Task<IActionResult> ProcessLessonReport([FromBody] LessonReportDto request)
        {
            var command = new ProcessLessonReportCommand(
                request.UserId,
                request.LessonId,
                request.Results);

            var result = await _mediator.Send(command);

            return result.Match(r => Ok(r.Value), CustomResults.Problem);
        }

        [HttpGet("reviews")]
        public async Task<IActionResult> GetDueReviews(
            [FromQuery] Guid userId,
            [FromQuery] int limit = 10,
            [FromQuery] DateTime? cursor = null)
        {
            var query = new GetDueReviewsQuery(userId, limit, cursor);
            var result = await _mediator.Send(query);

            return result.Match(r => Ok(r.Value), CustomResults.Problem);
        }

        [HttpPut("record/{recordId}/review")]
        public async Task<IActionResult> UpdateReview(
            Guid recordId,
            [FromBody] UpdateReviewRequest request)
        {
            var command = new UpdateReviewCommand(recordId, request.IsCorrect);
            var result = await _mediator.Send(command);

            return result.Match(r => Ok(r.Value), CustomResults.Problem);
        }
    }

    public class UpdateReviewRequest
    {
        public bool IsCorrect { get; set; }
    }
} 