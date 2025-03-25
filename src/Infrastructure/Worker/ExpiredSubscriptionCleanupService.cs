// File: Infrastructure/Worker/ExpiredSubscriptionCleanupService.cs

using System;
using System.Threading.Tasks;
using Application.Interface;
using Domain.Repositories;
using Microsoft.Extensions.Logging;
using Quartz;
using SharedKernel;

namespace Infrastructure.Worker
{
    public class ExpiredSubscriptionCleanupService : IJob
    {
        private readonly IUserRepository _userRepository;
        private readonly IApplicationDbContext _dbContext;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly ILogger<ExpiredSubscriptionCleanupService> _logger;

        public ExpiredSubscriptionCleanupService(
            IUserRepository userRepository,
            IApplicationDbContext dbContext,
            IDateTimeProvider dateTimeProvider,
            ILogger<ExpiredSubscriptionCleanupService> logger,
            ISubscriptionRepository subscriptionRepository)
        {
            _userRepository = userRepository;
            _dbContext = dbContext;
            _dateTimeProvider = dateTimeProvider;
            _logger = logger;
            _subscriptionRepository = subscriptionRepository;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("ExpiredSubsc    riptionCleanupService run at {time}", _dateTimeProvider.UtcNow);
            var now = _dateTimeProvider.UtcNow;
Console.WriteLine($"ExpiredSubscriptionCleanupService run at {now.ToShortDateString()}");
            var expiredUsers = await _userRepository.GetExpiredSubscriptions();
Console.WriteLine($"Cleaning up expired subscriptions for {expiredUsers.Count} users");
            using var transaction = await _dbContext.BeginTransactionAsync(context.CancellationToken);
            try{
                if (expiredUsers != null)
                {
                    foreach (var user in expiredUsers)
                    {
                        if (user.Subscription is not null)
                        {
                            var subscription = user.Subscription;
                            user.RemoveSubscription();
                            _subscriptionRepository.RemoveSubscription(subscription.Id);
                            _logger.LogInformation("Removed expired subscription for user {UserId}", user.Id);
                        }
                    }

                    await _dbContext.SaveChangesAsync(context.CancellationToken);
                    await transaction.CommitAsync(context.CancellationToken);
                }
                else
                {
                    _logger.LogInformation("No expired subscriptions found at {time}", now);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while cleaning up expired subscriptions");
                await transaction.RollbackAsync(context.CancellationToken);
            }
        }
    }
}