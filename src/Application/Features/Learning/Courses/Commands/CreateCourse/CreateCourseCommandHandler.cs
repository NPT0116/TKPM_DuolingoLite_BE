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
            var course = Course.Create(request.CreateCourseDto.Name, request.CreateCourseDto.Level, null);
            
            if(course.IsFailure)
            {
                return Result.Failure<Course>(course.Error);
            }
            await _courseRepository.CreateCourse(course.Value);
            await _context.SaveChangesAsync(cancellationToken);
            
            return course;
        }
    }
}