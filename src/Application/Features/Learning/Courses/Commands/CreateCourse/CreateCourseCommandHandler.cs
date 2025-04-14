using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions.Messaging;
using Application.Interface;
using Domain.Entities.Learning.Courses;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.Learning.Courses
{
    public class CreateCourseCommandHandler : ICommandHandler<CreateCourseCommand, Course>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IApplicationDbContext _context;
        public CreateCourseCommandHandler(ICourseRepository courseRepository, IApplicationDbContext context)
        {
            _courseRepository = courseRepository;
            _context = context;
        }
        public async Task<Result<Course>> Handle(CreateCourseCommand request, CancellationToken cancellationToken = default)
        {
            var courseCount = await _courseRepository.GetCourseCount();
            var level = courseCount + 1;

            // var courseAtLevel = await _courseRepository.GetCourseByLevel(level);
            // if(courseAtLevel != null)
            // {
            //     return Result.Failure<Course>(CourseError.CourseLevelMustBeUnique(level));
            // }

            var courseWithTheSameName = await _courseRepository.GetCourseByName(request.CreateCourseDto.Name);
            if(courseWithTheSameName != null)
            {
                return Result.Failure<Course>(CourseError.CourseNameMustBeUnique(request.CreateCourseDto.Name));
            }
            

            var course = Course.Create(request.CreateCourseDto.Name, level, null);
            
            if(course.IsFailure)
            {
                return Result.Failure<Course>(course.Error);
            }

            // if(courseCount > 0)
            // {
            //     var latestCourse = await _courseRepository.GetCourseByLevel(courseCount);
            //     if(latestCourse != null)
            //         latestCourse.SetNextCourse(course.Value);

            // }

            await _courseRepository.CreateCourse(course.Value);
            await _context.SaveChangesAsync(cancellationToken);
            
            return course;
        }
    }
}