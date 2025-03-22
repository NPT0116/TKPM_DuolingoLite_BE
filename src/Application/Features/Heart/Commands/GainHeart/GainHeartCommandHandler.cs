using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Heart.Dtos;
using Application.Interface;
using Domain.Entities.Users;
using Domain.Entities.Users.Constants;
using Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using SharedKernel;
using SharedKernel.Cache;

namespace Application.Features.Heart.Commands.GainHeart
{
    public class GainHeartCommandHandler : IRequestHandler<GainHeartCommand, Result<UserHeartDto>>
    {
        private readonly IDistributedCache _cache;
        private readonly IUserRepository _userRepository;
        private readonly IIdentityService _identityService;
        public GainHeartCommandHandler(
            IDistributedCache cache,
            IUserRepository userRepository,
            IIdentityService identityService)
        {
            _cache = cache;
            _userRepository = userRepository;
            _identityService = identityService;
        }
        public async Task<Result<UserHeartDto>> Handle(GainHeartCommand request, CancellationToken cancellationToken)
        {
            var currentUser = await _identityService.GetCurrentUserAsync();
            if(currentUser == null)
            {
                return Result.Failure<UserHeartDto>(UserError.Unauthorized());
            }

            var userId = currentUser.Id;
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

            if(heart >= HeartConstants.MAXIMUM_HEART)
            {
                return Result.Failure<UserHeartDto>(HeartError.CannotIncreaseHeartWhenAtMaximum);
            }

            await _cache.SetAsync<int>(heartKey, heart + 1);

            var userHeartDto = new UserHeartDto(userId, heart + 1);
            return Result.Success<UserHeartDto>(
                userHeartDto
            );
        }
    }
}