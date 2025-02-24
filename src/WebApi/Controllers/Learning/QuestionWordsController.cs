using Application.Features.Learning.QuestionWords.Queries.GetQuestionWordsByQuestionId;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Extensions;
using WebApi.Infrastructure;

namespace WebApi.Controllers.Learning
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionWordsController : ControllerBase
    {
        private readonly IMediator  _mediator;
        public QuestionWordsController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet("{questionId}")]
        public async Task<IActionResult> GetQuestionWordsByQuestionId([FromRoute]Guid questionId)
        {
            var query = new GetQuestionsWordByQuestionIdQuery(questionId);
            var result = await _mediator.Send(query);
            return result.Match(Ok, CustomResults.Problem);
        }
    }
}
