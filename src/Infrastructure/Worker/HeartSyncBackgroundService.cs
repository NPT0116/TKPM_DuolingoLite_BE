using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Interface;
using Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using StackExchange.Redis;

namespace Infrastructure.Worker
{
    public class HeartSyncBackgroundService : IJob
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<HeartSyncBackgroundService> _logger;

    public HeartSyncBackgroundService(
        IConnectionMultiplexer redis,
        IServiceScopeFactory scopeFactory,
        ILogger<HeartSyncBackgroundService> logger)
    {
        _redis = redis;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

        public async Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine("Background serivce run");
            var db = _redis.GetDatabase();

            try
            {

                // Get a Redis server to scan for keys.
                var server = _redis.GetServer(_redis.GetEndPoints().First());
                // Scan for keys matching the pattern for storing hearts.
                var keys = server.Keys(pattern: "user:heart:*").ToArray();

                foreach (var key in keys)
                {
                Console.WriteLine(key);

                    // Expecting key format: "user:{userId}:hearts"
                    var keyParts = key.ToString().Split(':');
                    if (keyParts.Length < 3)
                    {
                        continue;
                    }
                    var userId = keyParts[2];

                    // Retrieve the current heart count from Redis.
                    var heartValue = await db.HashGetAsync(key, "data");
                    Console.WriteLine("Current heart:" + heartValue);
                    if (!heartValue.HasValue || !int.TryParse(heartValue, out int hearts))
                    {
                        continue; // Skip if no valid data is found.
                    }

                    // Use a scoped service to update the persistent database.
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                        var dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
                        var userStats = await userRepository.GetUserStatsById(Guid.Parse(userId));
                        userStats?.UpdateHeart(hearts);
                        // Update the user's heart count in your database.
                        await dbContext.SaveChangesAsync();
                    }

                    _logger.LogInformation("Synchronized heart count for user {UserId}: {Hearts} hearts.", userId, hearts);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during heart synchronization.");
                // Optionally implement additional error handling or retries here.
            }
            
            
        }
    }
}