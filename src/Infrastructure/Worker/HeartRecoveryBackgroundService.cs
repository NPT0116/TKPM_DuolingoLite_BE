using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Heart.Commands.LoseHeart;
using Domain.Entities.Users.Constants;
using Domain.Repositories;
using Infrastructure.Services.Settings;
using MediatR;
using Microsoft.Extensions.Options;
using Quartz;
using SharedKernel.Cache;
using StackExchange.Redis;

namespace Infrastructure.Worker
{
    public class HeartRecoveryBackgroundService : IJob
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IUserRepository _userRepository;
        private readonly BackgroundSettings _backgroundSettings;
        public HeartRecoveryBackgroundService(
            IConnectionMultiplexer redis,
            IUserRepository userRepository,
            IOptions<BackgroundSettings> backgroundSettings)
        {
            _redis = redis;
            _userRepository = userRepository;
            _backgroundSettings = backgroundSettings.Value;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine("Refill service run");
            var db = _redis.GetDatabase();
            var cacheKey = Cache.HeartRefillPrefix;

            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var dueUsers = await db.SortedSetRangeByScoreAsync(cacheKey, double.NegativeInfinity, now);

            foreach(var userId in dueUsers)
            {
                var userProfile = await _userRepository.GetUserProfileById(Guid.Parse(userId));
                if(userProfile == null)
                {
                    continue;
                }
                if(userProfile.Subscription != null && !userProfile.Subscription.IsExpired)
                {
                    continue;
                }

                // Construct the key for the user's heart count.
                string heartKey = Cache.GetUserHeartKey(Guid.Parse(userId));

                // Retrieve the current heart count.
                var currentHeartValue = await db.HashGetAsync(heartKey, "data");
                int currentHeart = 0;
                if (currentHeartValue.HasValue && int.TryParse(currentHeartValue, out int parsed))
                {
                    currentHeart = parsed;
                }
                else
                {
                    var userStats = await _userRepository.GetUserStatsById(Guid.Parse(userId));
                    if(userStats != null)
                    {
                        currentHeart = userStats.Heart;
                    }
                }

                // Only increment if the user has fewer than the max allowed hearts.
                if (currentHeart < HeartConstants.MAXIMUM_HEART)
                {
                    // Increment the heart count.
                    await db.HashIncrementAsync(heartKey, "data");

                    // Optionally, re-schedule the next heart recovery if the user is still below maximum.
                    if (currentHeart + 1 < HeartConstants.MAXIMUM_HEART)
                    {
                        // For example, if the recovery interval is fixed (e.g., 4 hours):
                        var interval = _backgroundSettings.HeartRecoveryInterval;
                        double nextRecoveryTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + (double)interval * 60;
                        await db.SortedSetAddAsync(Cache.HeartRefillPrefix, userId, nextRecoveryTime);
                    }
                }
                else
                {
                    // Console.WriteLine("User {UserId} already has the maximum hearts.", userId);
                    await db.SortedSetRemoveAsync(Cache.HeartRefillPrefix, userId);
                }

                // Remove the current due entry.
            }
        }
    }
}