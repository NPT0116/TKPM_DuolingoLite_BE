using System;
using Application.Abstractions.Messaging;
using Application.Features.User.Queries.GetUserProfile;
using Application.Interface;
using Domain.Entities.Users;
using Domain.Repositories;
using MediatR;
using SharedKernel;

namespace Application.Features.User.Queries.GetAllUser;

public class GetAllUserQueryHandler :IRequestHandler<GetAllUserQuery, Result<PaginationResult<UserWithProfileResponseDto>>>
{

    private readonly IUserRepository _userRepository;
    private readonly IIdentityService _identityService;
    public GetAllUserQueryHandler(IUserRepository userRepository, IIdentityService identityService)
    {
        _userRepository = userRepository;
        _identityService = identityService;
    }

    public async Task<Result<PaginationResult<UserWithProfileResponseDto>>> Handle(GetAllUserQuery request, CancellationToken cancellationToken)
    {
        var queryParams = request.GetAllUserQueryParams;
        var users = await _userRepository.GetAllUsers(queryParams);
        if (users is null)
        {
            throw new Exception("Error while fetching get all users");
        }

        var items = new List<UserWithProfileResponseDto>();
        DateTime today = DateTime.UtcNow.Date;
        DateTime startOfWeek = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);
        DateTime endOfWeek = startOfWeek.AddDays(6);
        foreach (var user in users.Items)
        {
            var userActivities = await _userRepository.GetUserActivitiesWithinDateRangeByUserId(user.UserId, startOfWeek, endOfWeek);
            var userStats = await _userRepository.GetUserStatsById(user.UserId);
            var userRole = await _identityService.GetUserRole(user.UserId);
            var userProfile = new UserWithProfileResponseDto(
                user.Id,
                user.FirstName,
                user.LastName,
                user.Email,
                user.NickName,
                user.ProfileImage?.Url,
                userRole,
                user.Subscription,
                userActivities,
                userStats
            );
            items.Add(userProfile);
        }
        var response = new PaginationResult<UserWithProfileResponseDto>(
            items,
            users.PageNumber,
            users.PageSize,
            users.TotalCount
        );
            return response;
    }
}
