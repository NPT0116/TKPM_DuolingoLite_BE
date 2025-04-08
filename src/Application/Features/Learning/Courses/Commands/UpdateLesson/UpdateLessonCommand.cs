using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions.Messaging;
using Domain.Entities.Learning.Lessons;

namespace Application.Features.Learning.Courses.Commands.UpdateLesson;

public record UpdateLessonDto(string newTitle, int newXpEarned);

public record UpdateLessonCommand(Guid courseId, int lessonOrder, UpdateLessonDto dto) : ICommand<Lesson>;