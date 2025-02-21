using System;
using Application.Abstractions.Messaging;
using Domain.Entities.Learning.Courses;
using MediatR;

namespace Application.Features.Learning.Courses.Queries.GetActiveCourseWithAUser;

public record GetActiveCourseWithAUserResponseDto (
    Guid CourseId,
    int LessonOrder,
    Guid UserId
);

public record GetActiveCourseWithAUserQuery (Guid UserId) : IQuery<GetActiveCourseWithAUserResponseDto>;
