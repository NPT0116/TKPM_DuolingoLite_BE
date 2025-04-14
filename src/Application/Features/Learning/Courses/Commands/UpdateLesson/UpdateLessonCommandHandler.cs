using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions.Messaging;
using Application.Interface;
using Domain.Entities.Learning.Courses;
using Domain.Entities.Learning.Lessons;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.Learning.Courses.Commands.UpdateLesson
{
    public class UpdateLessonCommandHandler : ICommandHandler<UpdateLessonCommand, Lesson>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IApplicationDbContext _context;
        public UpdateLessonCommandHandler(
            ICourseRepository courseRepository,
            IApplicationDbContext context
        )
        {
            _courseRepository = courseRepository;
            _context = context;
        }
        public async Task<Result<Lesson>> Handle(UpdateLessonCommand request, CancellationToken cancellationToken)
        {
            var course = await _courseRepository.GetCourseById(request.courseId);
            if(course == null)
            {
                return Result.Failure<Lesson>(CourseError.CourseNotFound(request.courseId));
            }

            var lesson = course.GetLessonByOrder(request.lessonOrder);
            if(lesson == null)
            {
                return Result.Failure<Lesson>(CourseError.QuestionWithOrderNotFound(request.lessonOrder, request.courseId));
            }

            var updateXpEarned = lesson.SetXpEarned(request.dto.newXpEarned);
            if(updateXpEarned.IsFailure) return Result.Failure<Lesson>(updateXpEarned.Error);

            var updateTitle = lesson.SetTitle(request.dto.newTitle);
            if(updateTitle.IsFailure) return Result.Failure<Lesson>(updateTitle.Error);

            await _context.SaveChangesAsync();

            return Result.Success(lesson);
        }
    }
}