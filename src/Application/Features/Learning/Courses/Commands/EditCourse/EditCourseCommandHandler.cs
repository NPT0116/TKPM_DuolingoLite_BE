using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions.Messaging;
using Application.Interface;
using Domain.Entities.Learning.Courses;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.Learning.Courses.Commands.EditCourse
{
    public class EditCourseCommandHandler : ICommandHandler<EditCourseCommand, Course>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IApplicationDbContext _context;
        public EditCourseCommandHandler(
            ICourseRepository courseRepository,
            IApplicationDbContext context)
        {
            _courseRepository = courseRepository;
            _context = context;
        }
        public async Task<Result<Course>> Handle(EditCourseCommand request, CancellationToken cancellationToken)
        {
            var course = await _courseRepository.GetCourseById(request.courseId);
            if(course == null)
            {
                return Result.Failure<Course>(CourseError.CourseNotFound(request.courseId));
            }

            var courseWithTheSameName = await _courseRepository.GetCourseByName(request.dto.newName);
            if(courseWithTheSameName != null && courseWithTheSameName.Id != request.courseId)
            {
                return Result.Failure<Course>(CourseError.CourseNameMustBeUnique(request.dto.newName));
            }

            course.SetName(request.dto.newName);
            await _context.SaveChangesAsync();

            return Result.Success(course);
        }
    }
}