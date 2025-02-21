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
            var (title, xpEarned, order) = request.CreateLessonDto;
            var lesson = Lesson.Create(title, xpEarned, order);
            if(lesson.IsFailure)
            {
                return Result.Failure<Lesson>(lesson.Error);
            }
            var course = await _courseRepository.GetCourseById(request.CourseId);

            if(course == null)
            {
                return Result.Failure<Lesson>(CourseError.CourseNotFound(request.CourseId));
            }
            course.AddLesson(lesson.Value);
            await _context.SaveChangesAsync(cancellationToken);
            return lesson.Value;
        }
    }
}