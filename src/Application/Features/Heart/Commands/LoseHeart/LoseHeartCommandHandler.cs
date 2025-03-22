using Application.Features.Heart.Dtos;
using Application.Interface;
using Domain.Entities.Subscriptions;
using Domain.Entities.Users;
using Domain.Entities.Users.Constants;
using Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using SharedKernel;
using SharedKernel.Cache;

namespace Application.Features.Heart.Commands.LoseHeart
{
    public class LoseHeartCommandHandler : IRequestHandler<LoseHeartCommand, Result<UserHeartDto>>
    {
        private readonly IDistributedCache _cache;
        private readonly IUserRepository _userRepository;
        private readonly IIdentityService _identityService;
        public LoseHeartCommandHandler(
            IDistributedCache cache,
            IUserRepository userRepository,
            IIdentityService identityService)
        {
            _cache = cache;
            _userRepository = userRepository;
            _identityService = identityService;
        }
        public async Task<Result<UserHeartDto>> Handle(LoseHeartCommand request, CancellationToken cancellationToken)
        {
            var currentUser = await _identityService.GetCurrentUserAsync();
            if(currentUser == null)
            {
                return Result.Failure<UserHeartDto>(UserError.Unauthorized());
            }

            var userId = currentUser.Id;
            var userProfile = await _userRepository.GetUserProfileById(userId);
            if(userProfile == null)
            {
                return Result.Failure<UserHeartDto>(UserError.UserProfileNotFound(userId));
            }
            if(userProfile.Subscription != null && !userProfile.Subscription.IsExpired)
            {
                return Result.Failure<UserHeartDto>(HeartError.PremiumUserHeartDecreaseNotAllowedException);
            }
            var heartKey = Cache.GetUserHeartKey(userId);
            var isCachedHit = _cache.TryGetValue<int>(heartKey, out int heart);
            
            if(!isCachedHit)
            {
                var userStats = await _userRepository.GetUserStatsById(userId);
                if(userStats == null)
                {
                    return Result.Failure<UserHeartDto>(UserError.UserStatsNotFound(userId));
                }
                heart = userStats.Heart;
            }

            if(heart <= HeartConstants.MINIMUM_HEART)
            {
                return Result.Failure<UserHeartDto>(HeartError.CannotDeclineHeartWhenAtMinimum);
            }

            await _cache.SetAsync<int>(heartKey, heart - 1);

            var userHeartDto = new UserHeartDto(userId, heart - 1);
            return Result.Success<UserHeartDto>(
                userHeartDto
            );
        }
    }
}