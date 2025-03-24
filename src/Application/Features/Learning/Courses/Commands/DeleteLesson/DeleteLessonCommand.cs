using Application.Abstractions.Messaging;

namespace Application.Features.Learning.Courses.Commands.DeleteLesson;

public record DeleteLastLessonCommand(Guid courseId) : ICommand;