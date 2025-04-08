using Application.Abstractions.Messaging;

namespace Application.Features.Learning.Courses.Commands.DeleteLesson;

public record DeleteLessonCommand(Guid courseId, int lessonOrder) : ICommand;