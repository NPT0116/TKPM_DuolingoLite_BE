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
                await _context.SaveChangesAsync();
                await CalculateAndUpdateStreaksAsync(userId);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw new Exception("Failed to record activity");
            }
            
            // Calculate and update streaks

            return Result.Success(true);
        }

     public async Task<Result<(int currentStreak, int longestStreak)>> CalculateAndUpdateStreaksAsync(Guid userId)
{
    var today = _dateTimeProvider.UtcNow.Date;
    // Lấy hoạt động trong 30 ngày gần nhất
    var activities = await _userRepository.GetUserActivitiesWithinDateRangeByUserId(
        userId, today.AddDays(-30), today);

    var userStats = await _userRepository.GetUserStatsById(userId);
    if (userStats == null)
    {
        return Result.Failure<(int, int)>(new Error("UserStatsNotFound", "User stats not found", ErrorType.NotFound));
    }

    // Lấy danh sách các ngày hoạt động duy nhất và sắp xếp giảm dần
    var activeDates = activities
        .Where(a => a.IsActive)
        .Select(a => a.Date.Date)
        .Distinct()
        .OrderByDescending(d => d)
        .ToList();

    int currentStreak = 0;

    // Nếu không có hoạt động nào hoặc hoạt động mới nhất không phải hôm nay thì streak = 0
    if (activeDates.Count == 0 || activeDates.First() != today)
    {
        currentStreak = 0;
    }
    else
    {
        currentStreak = 1;
        var lastDate = today;
        // Lặp qua các ngày còn lại để tính streak liên tục
        for (int i = 1; i < activeDates.Count; i++)
        {
            var diff = (lastDate - activeDates[i]).Days;
            if (diff == 1) // Nếu cách nhau đúng 1 ngày
            {
                currentStreak++;
                lastDate = activeDates[i];
            }
            else
            {
                // Nếu có khoảng cách hơn 1 ngày, dừng lại
                break;
            }
        }
    }

    // Cập nhật streak cho userStats
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