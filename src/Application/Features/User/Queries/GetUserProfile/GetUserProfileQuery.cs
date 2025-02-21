using Domain.Entities.Subscriptions;
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
    Subscription Subscription
);
public record GetUserProfileQuery(Guid UserId) : IRequest<Result<UserWithProfileResponseDto>>;