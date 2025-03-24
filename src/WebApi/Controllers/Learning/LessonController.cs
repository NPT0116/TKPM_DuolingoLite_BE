using System;
using Application.Features.Learning.Lessons.Commands.AddQuestions;
using Application.Features.Learning.Lessons.Queries.GetListOfLessonFromCourseId;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using WebApi.Contracts.Requests;
using WebApi.Extensions;
using WebApi.Infrastructure;

namespace WebApi.Controllers.Learning;

[ApiController]
[Route("api/[controller]")]
public class LessonController: ControllerBase
{
    private readonly IMediator _mediator;
    public LessonController(IMediator mediator)
    {
        _mediator = mediator;
    }
    [HttpGet("{courseId}")]
    public async Task<IActionResult> GetListOfLessonFromCourseId ([FromRoute] Guid courseId)
    {
        var query = new GetListOfLessonFromCourseIdQuery(courseId);
        var result = await _mediator.Send(query);
        return result.Match(Ok, CustomResults.Problem);
    }

    [HttpPost("{id}/add-question")]
    public async Task<IActionResult> CreateQuestion([FromRoute] Guid id, [FromBody] QuestionDto dto)
    {
        var command = new AddQuestionCommand(id, dto);
        var result = await _mediator.Send(command);
        return result.Match(Ok, CustomResults.Problem);
    }
}
