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

namespace Infrastructure.EventHandlers
{
    public class UserLostHeartEventHandler : INotificationHandler<UserLostHeartEvent>
    {
        private readonly IDistributedCache _cache;
        private readonly BackgroundSettings _backgroundSettings;
        private readonly IUserRepository _userRepository;
        public UserLostHeartEventHandler(
            IDistributedCache cache,
            IOptions<BackgroundSettings> settings,
            IUserRepository userRepository)
        {
            _cache = cache;
            _backgroundSettings = settings.Value;
            _userRepository = userRepository;
        }

        public async Task Handle(UserLostHeartEvent notification, CancellationToken cancellationToken)
        {
            var userId = notification.userId;
            Console.WriteLine($"User with id {userId} has just lost heart");
            var user = await _userRepository.GetUserProfileById(userId);

            if(user == null) return;
            
            var userRefillCacheKey = Cache.GetUserHeartRefillScheduleKey(userId);
            var isCachedHit = await _cache.GetStringAsync(userRefillCacheKey);

            if(isCachedHit != null) return;
            var refillCron = _backgroundSettings.HeartRecoveryInterval;
            Console.WriteLine(refillCron);
            var cronExpression = CronExpression.Parse(refillCron, CronFormat.IncludeSeconds);
            DateTime now = DateTime.UtcNow;
            var nextOccurrence = cronExpression.GetNextOccurrence(now, TimeZoneInfo.Utc);
            if (!nextOccurrence.HasValue)
            {
                throw new InvalidOperationException("No next occurrence found for the given cron expression.");
            }

            // Get the occurrence after that (to determine the fixed interval)
            var subsequentOccurrence = cronExpression.GetNextOccurrence(nextOccurrence.Value, TimeZoneInfo.Utc);
            if (!subsequentOccurrence.HasValue)
            {
                throw new InvalidOperationException("No subsequent occurrence found for the given cron expression.");
            }

            // The interval between two successive occurrences is our fixed offset.
            TimeSpan interval = subsequentOccurrence.Value - nextOccurrence.Value;
            
            // Add this interval to the current time to get the scheduled time relative to now.
            DateTime scheduledTime = now.Add(interval);

            Console.WriteLine($"Current time: {now.ToString()}");
            Console.WriteLine($"Schedule time: {scheduledTime.ToString()}");


            await _cache.SetStringAsync(userRefillCacheKey, scheduledTime.ToString());
        }
    }
}