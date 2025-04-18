using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Learning.Courses;
using Application.Features.Learning.Courses.AddLesson;
using Application.Features.Learning.Courses.Commands.DeleteCourse;
using Application.Features.Learning.Courses.Commands.DeleteLesson;
using Application.Features.Learning.Courses.Commands.EditCourse;
using Application.Features.Learning.Courses.Commands.UpdateLesson;
using Application.Features.Learning.Courses.Commands.UserRegisterCourse;
using Application.Features.Learning.Courses.Queries.GetActiveCourseWithAUser;
using Application.Features.Learning.Courses.Queries.GetCourseDetail;
using Application.Features.Learning.Courses.Queries.GetCourseList;
using Application.Features.Learning.Lessons.Commands;
using Application.Interface;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        [Authorize(Roles ="Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateCourse([FromBody] CreateCourseDto createCourseDto, CancellationToken cancellationToken = default)
        {
            var command = new CreateCourseCommand(createCourseDto);
            var result = await _mediator.Send(command, cancellationToken);
            
            return result.Match(Ok, CustomResults.Problem);
        }

        [Authorize(Roles ="Admin")]
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

        [Authorize(Roles ="User,Admin")]
        [HttpPost("finish-lesson")]

        public async Task<IActionResult> FinishLesson([FromBody] UserFinishLessonRequestDto requestDto, CancellationToken cancellationToken = default)
        {   
            var command = new UserFinishLessonCommand(new UserFinishLessonRequestDto{
                CourseId = requestDto.CourseId,
            });
            var result = await _mediator.Send(command, cancellationToken);
            return result.Match(Ok, CustomResults.Problem);
        }
        [Authorize]
        [HttpPost("register")]
        public async Task<IActionResult> RegisterCourse([FromBody] UserRegisterCourseRequestDto userRegisterCourseDto, CancellationToken cancellationToken = default)
        {
            var command = new UserRegisterCourseCommand(userRegisterCourseDto);
            var result = await _mediator.Send(command, cancellationToken);
            return result.Match(Ok, CustomResults.Problem);
        }

        [Authorize(Roles ="Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse([FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            var command = new DeleteCourseCommand(id);
            var result = await _mediator.Send(command, cancellationToken);
            return result.Match(Ok, CustomResults.Problem);
        }

        [Authorize(Roles ="Admin")]
        [HttpDelete("{courseId}/lesson/{lessonOrder}")]
        public async Task<IActionResult> DeleteLastLesson(
            [FromRoute] Guid courseId,
            [FromRoute] int lessonOrder, 
            CancellationToken cancellationToken = default)
        {
            var command = new DeleteLessonCommand(courseId, lessonOrder);
            var result = await _mediator.Send(command, cancellationToken);
            return result.Match(Ok, CustomResults.Problem);
        }

        [Authorize(Roles ="Admin")]
        [HttpPut("{courseId}")]
        public async Task<IActionResult> UpdateCourse([FromRoute] Guid courseId, [FromBody] EditCourseDto editCourseDto)
        {
            var command = new EditCourseCommand(courseId, editCourseDto);
            var result = await _mediator.Send(command);
            return result.Match(Ok, CustomResults.Problem);
        }        

        [Authorize(Roles ="Admin")]
        [HttpPut("{courseId}/lesson/{lessonOrder}")]
        public async Task<IActionResult> UpdateLesson(
            [FromRoute] Guid courseId,
            [FromRoute] int lessonOrder,
            [FromBody] UpdateLessonDto updateLessonDto)
        {
            var command = new UpdateLessonCommand(courseId, lessonOrder, updateLessonDto);
            var result = await _mediator.Send(command);
            return result.Match(Ok, CustomResults.Problem);
        }
            
    }
}