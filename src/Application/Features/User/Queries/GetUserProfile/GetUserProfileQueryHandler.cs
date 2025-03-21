using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Interface;
using Domain.Entities.Users;
using Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using SharedKernel;
using SharedKernel.Cache;

namespace Application.Features.User.Queries.GetUserProfile
{
    public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, Result<UserWithProfileResponseDto>>
    {
        private readonly IIdentityService _identityService;
        private readonly IUserRepository _userRepository;
        private readonly IDistributedCache _cache;
        public GetUserProfileQueryHandler(
            IIdentityService identityService, 
            IUserRepository userRepository,
            IDistributedCache cache)
        {
            _identityService = identityService;
            _userRepository = userRepository;
            _cache = cache;
        }
        public async Task<Result<UserWithProfileResponseDto>> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
        {
            var user = await _identityService.GetCurrentUserAsync();
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
            var heartCacheKey = Cache.GetUserHeartKey(user.Id);
            var isCachedHit = _cache.TryGetValue<int>(heartCacheKey, out int heart);
            
            if(isCachedHit)
            {
                if(userStats.Heart != heart)
                {
                    userStats.UpdateHeart(heart);
                }
            }
            else
            {
                await _cache.SetAsync<int>(heartCacheKey, userStats.Heart);
            }
            

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