using Application.Features.Learning.Question.Queries.GetAQuestionFromLessionId;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Extensions;
using WebApi.Infrastructure;

namespace WebApi.Controllers.Learning
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionController : ControllerBase
    {
        private readonly IMediator _mediator;
        public QuestionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("questions/list-questions/{lessonId}")]
        public async Task<IActionResult> GetListOfQuestionsFromLessonId([FromRoute] Guid lessonId, [FromQuery] int questionOrder)
        {
            var query = new GetAQuestionFromLessonIdQuery(lessonId, questionOrder);
            var result = await _mediator.Send(query);
            return result.Match(Ok, CustomResults.Problem);
        }
    }
}
