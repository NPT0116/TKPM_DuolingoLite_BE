using System;
using Application.Abstractions.Messaging;
using Application.Interface;
using Domain.Entities.Users;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.User.Queries.GetUserActivityRange;

public class GetUserAcitivityRangeQueryHandler : IQueryHandler<GetUserAcitivityRangeQuery, List<GetUserActivityDto>>
{
    private readonly IIdentityService _identityService;
    private readonly IUserRepository _userRepository;
    public GetUserAcitivityRangeQueryHandler(IIdentityService identityService, IUserRepository userRepository)
    {
        _identityService = identityService;
        _userRepository = userRepository;
    }
    public async Task<Result<List<GetUserActivityDto>>> Handle(GetUserAcitivityRangeQuery request, CancellationToken cancellationToken)
    {
        var user = await _identityService.GetCurrentUserAsync();
        if (user is null)
        {
            return Result.Failure<List<GetUserActivityDto>>(UserError.UnauthorizedUser);
        }
        var userActivities = await _userRepository.GetUserActivitiesWithinDateRangeByUserId(user.Id, request.GetUserAcitivityRangeQueryParam.StartDate, request.GetUserAcitivityRangeQueryParam.EndDate);
        var response = userActivities.Select(activity => new GetUserActivityDto(user.Id, activity.Date, activity.Id)).ToList();
        return Result.Success(response);
    }
}
