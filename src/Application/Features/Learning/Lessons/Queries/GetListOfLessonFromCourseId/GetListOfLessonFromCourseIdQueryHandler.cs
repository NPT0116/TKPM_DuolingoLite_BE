using System;
using Application.Abstractions.Messaging;
using Domain.Entities.Learning.Courses;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.Learning.Lessons.Queries.GetListOfLessonFromCourseId;

public class GetListOfLessonFromCourseIdQueryHandler : IQueryHandler<GetListOfLessonFromCourseIdQuery, LessonsDto[]>
{
    private readonly ICourseRepository _courseRepository;
    public GetListOfLessonFromCourseIdQueryHandler(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }
    public async Task<Result<LessonsDto[]>> Handle(GetListOfLessonFromCourseIdQuery request, CancellationToken cancellationToken)
    {
        var course = await _courseRepository.GetCourseById(request.CourseId);
        if(course is null)
        {
            return Result.Failure<LessonsDto[]>(CourseError.CourseNotFound(request.CourseId));
        }
        var lessons = course.Lessons.Select(x => new LessonsDto(x.Id, x.Title, x.Order, x._questions.Count, x.XpEarned)).ToArray();
        return lessons;
    }
}
