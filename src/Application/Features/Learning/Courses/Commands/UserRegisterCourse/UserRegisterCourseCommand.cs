using System;
using Application.Abstractions.Messaging;
using SharedKernel;

namespace Application.Features.Learning.Courses.Commands.UserRegisterCourse;

public record UserRegisterCourseRequestDto
{
    public Guid CourseId { get; init; }
}

public record UserRegisterCourseResponseDto
{
    public Guid LearningProgressId { get; init; }
    public Guid UserId { get; init; }
    public Guid CourseId { get; init; }
    public string CourseName { get; init; }= string.Empty;
    }

public record UserRegisterCourseCommand(UserRegisterCourseRequestDto UserRegisterCourseDto) 
    : ICommand<UserRegisterCourseResponseDto>;