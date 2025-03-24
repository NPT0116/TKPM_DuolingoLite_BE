using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions.Messaging;
using Application.Interface;
using Domain.Entities.Learning.Courses;
using Domain.Repositories;
using MediatR;
using SharedKernel;

namespace Application.Features.Learning.Courses.Commands.DeleteLesson
{
    public class DeleteLastLessonCommandHandler : ICommandHandler<DeleteLastLessonCommand>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IApplicationDbContext _context;
        public DeleteLastLessonCommandHandler(
            ICourseRepository courseRepository,
            IApplicationDbContext context)
        {
            _courseRepository = courseRepository;
            _context = context;
        }

        async Task<Result> IRequestHandler<DeleteLastLessonCommand, Result>.Handle(DeleteLastLessonCommand request, CancellationToken cancellationToken)
        {
            var course = await _courseRepository.GetCourseById(request.courseId);
            if(course == null)
            {
                return Result.Failure(CourseError.CourseNotFound(request.courseId));
            }

            var lessonCount = course.Lessons.Count();
            if(lessonCount > 0)
            {
                var lastLesson = course
                    .Lessons
                    .OrderBy(l => l.Order)
                    .Last();
                course.RemoveLesson(lastLesson);
                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
            else
            {
                return Result.Failure(CourseError.NoLessonInCourse);
            }
        }
    }
}