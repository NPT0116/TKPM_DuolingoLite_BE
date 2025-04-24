using System;
using Application.Abstractions.Messaging;

namespace Application.Features.User.Queries.GetCourseUser;



public record GetCourseUserQueryResponseDto(
    

    Guid CourseId,
    int LessonOrder,
    int TotalLesson,
    string CourseName,
    bool IsCompleted
);

public record GetCourseUserQuery: IQuery<List<GetCourseUserQueryResponseDto>>;
