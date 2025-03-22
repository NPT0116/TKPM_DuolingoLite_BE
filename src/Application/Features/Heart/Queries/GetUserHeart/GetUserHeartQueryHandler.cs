using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Heart.Dtos;
using Application.Interface;
using Domain.Entities.Users;
using Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using SharedKernel;
using SharedKernel.Cache;

namespace Application.Features.Heart.Queries.GetUserHeart
{
    public class GetUserHeartQueryHandler : IRequestHandler<GetUserHeartQuery, Result<UserHeartDto>>
    {
        private readonly IDistributedCache _cache;
        private readonly IUserRepository _userRepository;
        private readonly IIdentityService _identityService;
        public GetUserHeartQueryHandler(
            IDistributedCache cache,
            IUserRepository userRepository,
            IIdentityService identityService)
        {
            _cache = cache;
            _userRepository = userRepository;
            _identityService = identityService;
        }

        public async Task<Result<UserHeartDto>> Handle(GetUserHeartQuery request, CancellationToken cancellationToken)
        {
            var currentUser = await _identityService.GetCurrentUserAsync();
            if(currentUser == null) return Result.Failure<UserHeartDto>(UserError.UnauthorizedUser);

            var userId = currentUser.Id;
            var heartKey = Cache.GetUserHeartKey(userId);
            var isCachedHit = _cache.TryGetValue<int>(heartKey, out int heart);

            if(isCachedHit) return Result.Success(new UserHeartDto(userId, heart));

            var userStats = await _userRepository.GetUserStatsById(userId);
            if(userStats == null) return Result.Failure<UserHeartDto>(UserError.UserStatsNotFound(userId));
            await _cache.SetAsync<int>(heartKey, userStats.Heart);
            return Result.Success(new UserHeartDto(userId, userStats.Heart));
        }
    }
}