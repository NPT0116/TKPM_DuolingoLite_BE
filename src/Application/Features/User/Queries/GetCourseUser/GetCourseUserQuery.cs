using System;
using Application.Abstractions.Messaging;

namespace Application.Features.User.Queries.GetCourseUser;



public record GetCourseUserQueryResponseDto(
    

    Guid CourseId,
    int LessonOrder,
    int TotalLesson,
    string CourseName
);

public record GetCourseUserQuery: IQuery<List<GetCourseUserQueryResponseDto>>;
