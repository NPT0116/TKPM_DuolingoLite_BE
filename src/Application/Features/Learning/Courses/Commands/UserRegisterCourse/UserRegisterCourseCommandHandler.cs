using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.Messaging;
using Application.Interface;
using Domain.Entities.Learning.Courses;
using Domain.Entities.Learning.LearningProgresses;
using Domain.Entities.Users;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.Learning.Courses.Commands.UserRegisterCourse;

public class UserRegisterCourseCommandHandler : ICommandHandler<UserRegisterCourseCommand, UserRegisterCourseResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly ILearningProgressRepository _learningProgressRepository;
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;

    public UserRegisterCourseCommandHandler(
        IUserRepository userRepository,
        ICourseRepository courseRepository,
        ILearningProgressRepository learningProgressRepository,
        IApplicationDbContext context,
        IIdentityService identityService)
    {
        _userRepository = userRepository;
        _courseRepository = courseRepository;
        _learningProgressRepository = learningProgressRepository;
        _context = context;
        _identityService = identityService;
    }

    public async Task<Result<UserRegisterCourseResponseDto>> Handle(
        UserRegisterCourseCommand request, 
        CancellationToken cancellationToken)
    {
        // Validate user exists
        var user = await _identityService.GetCurrentUserAsync();
        if (user == null)
        {
            return Result.Failure<UserRegisterCourseResponseDto>(
                UserError.Unauthorized());
        }

        // Validate course exists
        var course = await _courseRepository.GetCourseById(request.UserRegisterCourseDto.CourseId);
        if (course == null)
        {
            return Result.Failure<UserRegisterCourseResponseDto>(
                CourseError.CourseNotFound(request.UserRegisterCourseDto.CourseId));
        }

        // Check if user is already enrolled in the course
        var existingProgress = await _learningProgressRepository.GetLearningProgressByUserIdAndCourseIdAsync(
            user.Id, request.UserRegisterCourseDto.CourseId);

        if (existingProgress != null)
        {
            return Result.Failure<UserRegisterCourseResponseDto>(
                LearningProgressError.UserAlreadyEnrolledInCourse());
        }

        // Create a new learning progress entity
        var learningProgress = LearningProgress.Create(
            user.Id,
            course);  

        if (learningProgress.IsFailure)
        {
            return Result.Failure<UserRegisterCourseResponseDto>(learningProgress.Error);
        }

        using var transaction = await _context.BeginTransactionAsync(cancellationToken);
        try
        {
            // Save the learning progress
            await _learningProgressRepository.AddAsync(learningProgress.Value);


            // Save changes and commit transaction
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            // Return success response
            return Result.Success(new UserRegisterCourseResponseDto
            {
                LearningProgressId = learningProgress.Value.Id,
                UserId = user.Id,
                CourseId = request.UserRegisterCourseDto.CourseId,
                CourseName = course.Name
                });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw new Exception("An error occurred while registering user to course", ex);
        }
    }
}