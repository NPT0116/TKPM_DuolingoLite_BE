using Application.Abstractions.Messaging;
using Application.Interface;
using Domain.Entities.User;
using Domain.Entities.Users;
using Domain.Repositories;
using SharedKernel;
using System;

namespace Application.Features.User.Commands.Register;

public class UserRegisterCommandHandler : ICommandHandler<UserRegisterCommand, Guid>
{
    private readonly IIdentityService _identityService;
    private readonly IUserRepository _userRepository;
    private readonly IApplicationDbContext _context;
    public UserRegisterCommandHandler(IIdentityService identityService, IUserRepository userRepository, IApplicationDbContext context)
    {
        _identityService = identityService;
        _userRepository = userRepository;
        _context = context;
    }

    public async Task<Result<Guid>> Handle(UserRegisterCommand request, CancellationToken cancellationToken)
    {
        if (await _identityService.UseEmailExistsAsync(request.UserRegisterDto.Email))
        {
            return Result.Failure<Guid>(UserError.EmailNotUnique) as Result<Guid>;
        }
        if (await _identityService.UserNameExistsAsync(request.UserRegisterDto.UserName))
        {
            return Result.Failure<Guid>(UserError.UserNameNotUnique);
        }
        var result = await _identityService.CreateUserAsync(
            request.UserRegisterDto.FirstName, 
            request.UserRegisterDto.LastName, 
            request.UserRegisterDto.Email, 
            request.UserRegisterDto.UserName, 
            request.UserRegisterDto.Password);

        if (result.Result.IsFailure)
        {
            return Result.Failure<Guid>(result.Result.Error);
        }

        var userId = result.UserId;

        var userActivity = UserActivity.Create(userId, DateTime.UtcNow, true);
        if (userActivity.IsFailure)
        {
            return Result.Failure<Guid>(userActivity.Error);
        }

        var userStats = UserStats.Create(userId);
        if (userStats.IsFailure)
        {
            return Result.Failure<Guid>(userStats.Error);
        }

        var userProfile = UserProfile.Create(userId, request.UserRegisterDto.Email, request.UserRegisterDto.UserName, request.UserRegisterDto.FirstName, request.UserRegisterDto.LastName, null, null);
        if (userProfile.IsFailure)
        {
            return Result.Failure<Guid>(userProfile.Error);
        }

        await _userRepository.CreateUserActivity(userActivity.Value);
        await _userRepository.CreateUserStats(userStats.Value);
        await _userRepository.CreateUserProfile(userProfile.Value);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(userId);
    }
}
