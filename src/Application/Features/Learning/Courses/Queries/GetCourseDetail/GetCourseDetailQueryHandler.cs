using System;
using Application.Abstractions.Messaging;
using Domain.Entities.Learning.Courses;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.Learning.Courses.Queries.GetCourseDetail;

public class GetCourseDetailQueryHandler : IQueryHandler<GetCourseDetailQuery, CourseDetailDto>
{
    private readonly ICourseRepository _courseRepository;
    public GetCourseDetailQueryHandler(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }
    public async Task<Result<CourseDetailDto>> Handle(GetCourseDetailQuery request, CancellationToken cancellationToken)
    {
        var course = await _courseRepository.GetCourseById(request.CourseId);
        if (course is null)
        {
            return Result.Failure<CourseDetailDto>(CourseError.CourseNotFound(request.CourseId));
        }
        Console.WriteLine(course.Name);
        return new CourseDetailDto(
            course.Id,
            course.Name,
            course.NextCourse?.Id ?? Guid.Empty // Trả về một giá trị hợp lệ nếu NextCourse là null
        );

    }
}
