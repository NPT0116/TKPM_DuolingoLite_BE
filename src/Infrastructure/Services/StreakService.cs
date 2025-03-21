using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interface;
using Application.Interface;
using Domain.Entities.Users;
using Domain.Repositories;
using SharedKernel;

namespace Infrastructure.Services
{
    public class StreakService : IStreakService
    {
        private readonly IUserRepository _userRepository;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IApplicationDbContext  _context;

        public StreakService(IUserRepository userRepository, IDateTimeProvider dateTimeProvider, IApplicationDbContext context)
        {
            _userRepository = userRepository;
            _dateTimeProvider = dateTimeProvider;
            _context = context;
        }


        public async Task<Result<bool>> RecordActivityAsync(Guid userId)
        {
            var today = _dateTimeProvider.UtcNow.Date;
            
            // Get today's activity if it exists
            var todayActivities = await _userRepository.GetUserActivitiesWithinDateRangeByUserId(
                userId, today, today);

            // If we already have an activity for today, return true without creating a new one
            if (todayActivities.Any(a => a.IsActive))
            {
                return Result.Success(true);
            }

            // Create new activity for today
            var activityResult = UserActivity.Create(userId, today, true);
            if (activityResult.IsFailure)
            {
                return Result.Failure<bool>(activityResult.Error);
            }
            using var transaction = await _context.BeginTransactionAsync();
            try
            {
                await _userRepository.CreateUserActivity(activityResult.Value);
                await CalculateAndUpdateStreaksAsync(userId);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return Result.Failure<bool>(new Error("ActivityCreationFailed", "Failed to create activity", ErrorType.InternalError));
            }
            
            // Calculate and update streaks

            return Result.Success(true);
        }

        public async Task<Result<(int currentStreak, int longestStreak)>> CalculateAndUpdateStreaksAsync(Guid userId)
        {
            var today = _dateTimeProvider.UtcNow.Date;
            var startDate = today.AddDays(-30); // Look back 30 days to calculate streak

            var activities = await _userRepository.GetUserActivitiesWithinDateRangeByUserId(
                userId, startDate, today);

            var userStats = await _userRepository.GetUserStatsById(userId);
            if (userStats == null)
            {
                return Result.Failure<(int, int)>(new Error("UserStatsNotFound", "User stats not found", ErrorType.NotFound));
            }

            var orderedActivities = activities
                .Where(a => a.IsActive)
                .OrderByDescending(a => a.Date)
                .ToList();

            int currentStreak = 0;
            var lastDate = today;

            // Calculate current streak
            foreach (var activity in orderedActivities)
            {
                var daysDiff = (lastDate - activity.Date.Date).Days;
                
                if (daysDiff <= 1) // Consecutive day or same day
                {
                    currentStreak++;
                    lastDate = activity.Date.Date;
                }
                else // Streak broken
                {
                    break;
                }
            }

            // If no activity today, and last activity was yesterday, reduce streak by 1
            if (!orderedActivities.Any(a => a.Date.Date == today) && 
                orderedActivities.Any(a => a.Date.Date == today.AddDays(-1)))
            {
                currentStreak--;
            }

            // Update streaks in user stats
            userStats.UpdateCurrentStreak(currentStreak);
            if (currentStreak > userStats.LongestStreak)
            {
                userStats.UpdateLongestStreak(currentStreak);
            }

            await _userRepository.UpdateUserStats(userStats);
            return Result.Success((currentStreak, userStats.LongestStreak));
        }

        public async Task<Result<(int currentStreak, int longestStreak, bool hasActivityToday)>> GetStreakStatusAsync(Guid userId)
        {
            var today = _dateTimeProvider.UtcNow.Date;
            
            var userStats = await _userRepository.GetUserStatsById(userId);
            if (userStats == null)
            {
                return Result.Failure<(int, int, bool)>(new Error("UserStatsNotFound", "User stats not found", ErrorType.NotFound));
            }

            var todayActivities = await _userRepository.GetUserActivitiesWithinDateRangeByUserId(
                userId, today, today);

            bool hasActivityToday = todayActivities.Any(a => a.IsActive);

            return Result.Success((userStats.CurrentStreak, userStats.LongestStreak, hasActivityToday));
        }
    }
} 