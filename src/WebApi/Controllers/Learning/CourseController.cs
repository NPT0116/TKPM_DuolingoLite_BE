using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Learning.Courses;
using Application.Features.Learning.Courses.AddLesson;
using Application.Features.Learning.Courses.Commands.UserRegisterCourse;
using Application.Features.Learning.Courses.Queries.GetActiveCourseWithAUser;
using Application.Features.Learning.Courses.Queries.GetCourseDetail;
using Application.Features.Learning.Courses.Queries.GetCourseList;
using Application.Features.Learning.Lessons.Commands;
using Application.Interface;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Extensions;
using WebApi.Infrastructure;

namespace WebApi.Controllers.Learning
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourseController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IIdentityService _identityService;
        public CourseController(IMediator mediator, IIdentityService identityService)
        {
            _mediator = mediator;
            _identityService = identityService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCourses(CancellationToken cancellationToken = default)
        {
            var query = new GetCourseListQuery();
            var result = await _mediator.Send(query, cancellationToken);
            return result.Match(Ok, CustomResults.Problem);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCourse([FromBody] CreateCourseDto createCourseDto, CancellationToken cancellationToken = default)
        {
            var command = new CreateCourseCommand(createCourseDto);
            var result = await _mediator.Send(command, cancellationToken);
            
            return result.Match(Ok, CustomResults.Problem);
        }

        [HttpPost("{id}/lesson")]
        public async Task<IActionResult> AddLesson([FromRoute] Guid id, [FromBody] CreateLessonDto createLessonDto, CancellationToken cancellationToken = default)
        {
            var command = new AddLessonCommand(id, createLessonDto);
            var result = await _mediator.Send(command, cancellationToken);
            return result.Match(Ok, CustomResults.Problem);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourseDetail([FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            var query = new GetCourseDetailQuery(id);
            var result = await _mediator.Send(query, cancellationToken);
            return result.Match(Ok, CustomResults.Problem);
        }
        [HttpGet ("current/{userId}")]
        public async Task<IActionResult> GetActiveCourseWithAUser([FromRoute] Guid userId, CancellationToken cancellationToken = default)
        {
            var query = new GetActiveCourseWithAUserQuery(userId);
            var result = await _mediator.Send(query, cancellationToken);
            return result.Match(Ok, CustomResults.Problem);
        }
        [HttpPost("finish-lesson")]

        public async Task<IActionResult> FinishLesson([FromBody] UserFinishLessonRequestDto requestDto, CancellationToken cancellationToken = default)
        {   
            var command = new UserFinishLessonCommand(new UserFinishLessonRequestDto{
                CourseId = requestDto.CourseId,
            });
            var result = await _mediator.Send(command, cancellationToken);
            return result.Match(Ok, CustomResults.Problem);
        }
        [HttpPost("register")]
        public async Task<IActionResult> RegisterCourse([FromBody] UserRegisterCourseRequestDto userRegisterCourseDto, CancellationToken cancellationToken = default)
        {
            var command = new UserRegisterCourseCommand(userRegisterCourseDto);
            var result = await _mediator.Send(command, cancellationToken);
            return result.Match(Ok, CustomResults.Problem);
        }        
    }
}