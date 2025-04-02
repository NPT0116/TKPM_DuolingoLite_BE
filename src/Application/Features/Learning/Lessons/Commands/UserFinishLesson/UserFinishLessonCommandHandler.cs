using System;
using Application.Abstractions.Messaging;
using Application.Interface;
using Domain.Entities.Learning.Courses;
using Domain.Entities.Learning.LearningProgresses;
using Domain.Entities.Learning.Lessons;
using Domain.Entities.Ranking;
using Domain.Entities.Users;
using Domain.Repositories;
using MediatR;
using SharedKernel;

namespace Application.Features.Learning.Lessons.Commands.UserFinishLesson;

public class UserFinishLessonCommandHandler : ICommandHandler<UserFinishLessonCommand, UserFinishLessonResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly ILearningProgressRepository _learningProgressRepository;
    private readonly ILessonRepository _lessonRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;
    private readonly IPublisher _publisher;
    public UserFinishLessonCommandHandler(IUserRepository userRepository, ILearningProgressRepository learningProgressRepository, ILessonRepository lessonRepository, ICourseRepository courseRepository, IApplicationDbContext context, IIdentityService identityService,
        IPublisher publisher)
    {
        _userRepository = userRepository;
        _learningProgressRepository = learningProgressRepository;
        _lessonRepository = lessonRepository;
        _courseRepository = courseRepository;
        _context = context;
        _identityService = identityService;
        _publisher = publisher;
    }


    public async Task<Result<UserFinishLessonResponseDto>> Handle(UserFinishLessonCommand request, CancellationToken cancellationToken)
    {
        var user = await _identityService.GetCurrentUserAsync();
        if (user == null)
        {
            return Result.Failure<UserFinishLessonResponseDto>(UserError.Unauthorized());
        }
        var userStats = await _userRepository.GetUserStatsById(user.Id);
        if (userStats == null)
        {
            return Result.Failure<UserFinishLessonResponseDto>(UserError.NotFound(user.Id));
        }
        var learningProgress = await _learningProgressRepository.GetLearningProgressByUserIdAsync(user.Id);
        if (learningProgress == null)
        {
            return Result.Failure<UserFinishLessonResponseDto>(LearningProgressError.LearningProgresssForUserNotFound(user.Id));
        }

        var course = await _courseRepository.GetCourseById(request.UserFinishLessonDto.CourseId);
        if (course == null)
        {
            return Result.Failure<UserFinishLessonResponseDto>(CourseError.CourseNotFound(request.UserFinishLessonDto.CourseId));
        }
        if (learningProgress.IsCompleted)
        {
            return Result.Success(new UserFinishLessonResponseDto
            {
                ExperiencePoint = userStats.ExperiencePoint,
                XpEarned = 0
            });
        }
        using var transaction = await _context.BeginTransactionAsync(cancellationToken);
        try {
            if (learningProgress.LessonOrder + 1 > course.Lessons.Count)
            {
                learningProgress.MarkAsCompleted();
            }
            else
            {
                learningProgress.UpdateLessonOrder(learningProgress.LessonOrder + 1);
            }
            userStats.AddExperiencePoints(course.GetLessonByOrder(learningProgress.LessonOrder).XpEarned);
            await _userRepository.UpdateUserStats(userStats);
            await _learningProgressRepository.UpdateAsync(learningProgress);
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            // Update cache for user ranking
            var updateRankingEvent = new UpdateRankingEvent(userStats.ExperiencePoint,user.Id);
            await _publisher.Publish(updateRankingEvent, cancellationToken);

        }
        catch (Exception e)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw new Exception(e.Message);
        }
        return new UserFinishLessonResponseDto
        {
            ExperiencePoint = userStats.ExperiencePoint,
            XpEarned = course.GetLessonByOrder(learningProgress.LessonOrder).XpEarned
        };

    }
}
