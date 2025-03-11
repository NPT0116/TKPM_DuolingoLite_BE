using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Users;
using Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using SharedKernel;
using SharedKernel.Cache;

namespace Application.Features.Heart.Queries.GetUserHeart
{
    public class GetUserHeartQueryHandler : IRequestHandler<GetUserHeartQuery, Result<UserHeartResponseDto>>
    {
        private readonly IDistributedCache _cache;
        private readonly IUserRepository _userRepository;
        public GetUserHeartQueryHandler(
            IDistributedCache cache,
            IUserRepository userRepository)
        {
            _cache = cache;
            _userRepository = userRepository;
        }

        public async Task<Result<UserHeartResponseDto>> Handle(GetUserHeartQuery request, CancellationToken cancellationToken)
        {
            var userId = request.userId;
            var heartKey = Cache.GetUserHeartKey(userId);
            var isCachedHit = _cache.TryGetValue<int>(heartKey, out int heart);

            if(isCachedHit) return Result.Success(new UserHeartResponseDto(userId, heart));

            var userStats = await _userRepository.GetUserStatsById(userId);
            if(userStats == null) return Result.Failure<UserHeartResponseDto>(UserError.UserStatsNotFound(userId));
            return Result.Success(new UserHeartResponseDto(userId, userStats.Heart));
        }
    }
}