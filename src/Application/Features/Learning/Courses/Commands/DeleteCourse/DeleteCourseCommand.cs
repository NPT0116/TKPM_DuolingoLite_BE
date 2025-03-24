using Application.Abstractions.Messaging;
using SharedKernel;

namespace Application.Features.Learning.Courses.Commands.DeleteCourse;

public record DeleteCourseCommand(Guid courseId) : ICommand;