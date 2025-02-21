using System;
using Application.Abstractions.Messaging;

namespace Application.Features.Learning.Lessons.Queries.GetListOfLessonFromCourseId;

public record LessonsDto(Guid Id, string Title, int Order, int QuestionCount, int XpEarned);
public record GetListOfLessonFromCourseIdQuery (Guid CourseId) : IQuery<LessonsDto[]>;