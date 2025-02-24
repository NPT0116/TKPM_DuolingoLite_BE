using Domain.Entities.Subscriptions;
using Domain.Entities.Users;
using MediatR;
using SharedKernel;

namespace Application.Features.User.Queries.GetUserProfile;

public record UserWithProfileResponseDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string NickName,
    string ProfileImageUrl,
    Subscription Subscription,
    List<UserActivity> UserActivities,
    UserStats UserStats
);
public record GetUserProfileQuery() : IRequest<Result<UserWithProfileResponseDto>>;