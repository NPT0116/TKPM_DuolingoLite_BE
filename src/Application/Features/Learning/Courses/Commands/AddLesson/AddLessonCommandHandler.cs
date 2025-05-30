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

namespace Application.Features.Learning.Courses.AddLesson
{
    public class AddLessonCommandHandler : ICommandHandler<AddLessonCommand, Lesson>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICourseRepository _courseRepository;
        public AddLessonCommandHandler(IApplicationDbContext context, ICourseRepository courseRepository)
        {
            _context = context;
            _courseRepository = courseRepository;
        }
        public async Task<Result<Lesson>> Handle(AddLessonCommand request, CancellationToken cancellationToken = default)
        {
            var course = await _courseRepository.GetCourseById(request.CourseId);
            if(course == null)
            {
                return Result.Failure<Lesson>(CourseError.CourseNotFound(request.CourseId));
            }

            var userTakingCourseCount = await _courseRepository.GetUserTakingCourseCount(request.CourseId);
            if(userTakingCourseCount > 0)
            {
                return Result.Failure<Lesson>(CourseError.CannotAddLessonToTakenCourse(request.CourseId));
            }

            var (title, xpEarned) = request.CreateLessonDto;
            var order = course.Lessons.Count + 1; 
            var lesson = Lesson.Create(title, xpEarned, order);
            if(lesson.IsFailure)
            {
                return Result.Failure<Lesson>(lesson.Error);
            }

            course.AddLesson(lesson.Value);
            await _context.SaveChangesAsync(cancellationToken);
            return lesson.Value;
        }
    }
}