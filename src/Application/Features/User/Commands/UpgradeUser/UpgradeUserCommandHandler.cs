using System;
using Application.Abstractions.Messaging;
using Application.Interface;
using Domain.Entities.Subscriptions;
using Domain.Entities.Users;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.User.Commands.UpgradeUser;

public class UpgradeUserCommandHandler : ICommandHandler<UpgradeUserCommand, bool>
{
    private readonly IUserRepository _userRepository;
    private readonly IIdentityService _identityService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IApplicationDbContext _dbContext;
    public UpgradeUserCommandHandler(IUserRepository userRepository, IIdentityService identityService, IDateTimeProvider dateTimeProvider, IApplicationDbContext applicationDbContext)
    {
        _userRepository = userRepository;
        _identityService = identityService;
        _dateTimeProvider = dateTimeProvider;
        _dbContext = applicationDbContext;
    }

    public async Task<Result<bool>> Handle(UpgradeUserCommand request, CancellationToken cancellationToken)
    {
        if (request.UpgradeUserCommandDto.DurationInMonth <= 0)
            return Result.Failure<bool>(SubscriptionError.InvalidSubscriptionMonth(request.UpgradeUserCommandDto.DurationInMonth));
        if (request.UpgradeUserCommandDto.Price <= 0)
            return Result.Failure<bool>(SubscriptionError.NegativePrice(request.UpgradeUserCommandDto.Price));
        var userDto = await _identityService.GetCurrentUserAsync();
        if (userDto is null)
            return Result.Failure<bool>(UserError.Unauthorized());
        var userProfile = await _userRepository.GetUserProfileById(userDto.Id);
        if (userProfile is null)
            return Result.Failure<bool>(UserError.UserProfileNotFound(userDto.Id));
        var subscription = userProfile.Subscription;
        if (subscription is not null && !subscription.IsExpired)
            return  Result.Failure<bool>(SubscriptionError.AlreadyInSubscription());
        var subscriptionResult = Subscription.Create(
            request.UpgradeUserCommandDto.DurationInMonth,
            request.UpgradeUserCommandDto.Price,
            _dateTimeProvider
        );
        if (subscriptionResult.IsFailure)
            throw new Exception("Can't create subscription");

        userProfile.UpdateSubscription(subscriptionResult.Value);
        await _userRepository.UpdateUserProfile(userProfile);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success(true);
    }
}
