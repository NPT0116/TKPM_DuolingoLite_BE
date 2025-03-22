using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus.Extensions.UnitedKingdom;
using Cronos;
using Domain.Entities.Users.Events;
using Domain.Repositories;
using Infrastructure.Services.Settings;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using SharedKernel.Cache;
using StackExchange.Redis;

namespace Infrastructure.EventHandlers
{
    public class UserLostHeartEventHandler : INotificationHandler<UserLostHeartEvent>
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly BackgroundSettings _backgroundSettings;
        private readonly IUserRepository _userRepository;
        public UserLostHeartEventHandler(
            IConnectionMultiplexer redis,
            IOptions<BackgroundSettings> settings,
            IUserRepository userRepository)
        {
            _redis = redis;
            _backgroundSettings = settings.Value;
            _userRepository = userRepository;
        }

        public async Task Handle(UserLostHeartEvent notification, CancellationToken cancellationToken)
        {
            var userId = notification.userId;
            Console.WriteLine($"User with id {userId} has just lost heart");
            var user = await _userRepository.GetUserProfileById(userId);

            if(user == null) return;
            var db = _redis.GetDatabase();
            
            var userRefillCacheKey = Cache.GetUserHeartRefillScheduleKey(userId);
            
            var existingScore = await db.SortedSetScoreAsync(Cache.HeartRefillPrefix, userRefillCacheKey);
            if (existingScore.HasValue)
            {
                // User is already scheduled for recovery.
                return;
            }
            var interval = _backgroundSettings.HeartRecoveryInterval;
            
            // Add this interval to the current time to get the scheduled time relative to now.
            var now = DateTime.UtcNow;
            DateTime scheduledTime = now.AddMinutes(interval);

            Console.WriteLine($"Current time: {now.ToString()}");
            Console.WriteLine($"Schedule time: {scheduledTime.ToString()}");


             long scheduledUnixTime = new DateTimeOffset(scheduledTime).ToUnixTimeSeconds();

            // Add the user to the sorted set with the scheduled recovery time as the score.
            await db.SortedSetAddAsync(Cache.HeartRefillPrefix, userId.ToString(), scheduledUnixTime);
        }
    }
}