using System;
using Application.Features.Learning.Lessons.Queries.GetListOfLessonFromCourseId;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
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
}
