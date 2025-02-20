using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions.Messaging;
using Domain.Entities.Learning.Lessons;

namespace Application.Features.Learning.Courses.AddLesson;

public record CreateLessonDto(string Title, int XpEarned, int Order, Guid CourseId);
public record AddLessonCommand(CreateLessonDto CreateLessonDto) : ICommand<Lesson>;
