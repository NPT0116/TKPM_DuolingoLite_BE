using System;
using Application.Abstractions.Messaging;

namespace Application.Features.Learning.Courses.Queries.GetCourseDetail;


public record CourseDetailDto (
Guid CourseId,
string Name,
Guid NextCourseId

);
public record GetCourseDetailQuery (Guid CourseId) : IQuery<CourseDetailDto>;