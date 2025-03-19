using System;
using System.Windows.Input;
using Application.Abstractions.Messaging;

namespace Application.Features.Learning.Lessons.Commands;


public class UserFinishLessonRequestDto
{
        public Guid CourseId { get; set; }
}
public class UserFinishLessonResponseDto
{
        public int ExperiencePoint { get; set; }
        public int XpEarned { get;  set; }
}
public record UserFinishLessonCommand (UserFinishLessonRequestDto UserFinishLessonDto): ICommand<UserFinishLessonResponseDto>;
