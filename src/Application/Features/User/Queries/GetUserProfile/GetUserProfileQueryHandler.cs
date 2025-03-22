using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Interface;
using Domain.Entities.Users;
using Domain.Repositories;
using MediatR;
using SharedKernel;

namespace Application.Features.User.Queries.GetUserProfile
{
    public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, Result<UserWithProfileResponseDto>>
    {
        private readonly IIdentityService _identityService;
        private readonly IUserRepository _userRepository;
        public GetUserProfileQueryHandler(IIdentityService identityService, IUserRepository userRepository)
        {
            _identityService = identityService;
            _userRepository = userRepository;
        }
        public async Task<Result<UserWithProfileResponseDto>> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
        {
            
            var user = await _identityService.GetCurrentUserAsync();
            // Console.WriteLine("User: " + user);
            if (user is null)
            {
                return Result.Failure<UserWithProfileResponseDto>(UserError.UnauthorizedUser);
            }
        
            var userProfile = await _userRepository.GetUserProfileById(user.Id);
            if (userProfile is null)
            {
                return Result.Failure<UserWithProfileResponseDto>(UserError.UserProfileNotFound(user.Id));
            }

            var userStats = await _userRepository.GetUserStatsById(user.Id);

            DateTime today = DateTime.UtcNow.Date;
            DateTime startOfWeek = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);
            DateTime endOfWeek = startOfWeek.AddDays(6);

            var userActivities = await _userRepository.GetUserActivitiesWithinDateRangeByUserId(user.Id, startOfWeek, endOfWeek);

            return new UserWithProfileResponseDto(
                user.Id, 
                user.FirstName, 
                user.LastName, 
                user.Email, 
                userProfile.NickName, 
                userProfile.ProfileImage?.Url, 
                userProfile.Subscription,
                userActivities,
                userStats
            );
        }
    }
}