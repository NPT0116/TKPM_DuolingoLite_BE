using System;
using SharedKernel;

namespace Application.Common.Interface;

    public interface IStreakService
    {
        /// <summary>
        /// Records a user activity for the current day if not already recorded
        /// </summary>
        Task<Result<bool>> RecordActivityAsync(Guid userId);

        /// <summary>
        /// Calculates and updates the user's current and longest streaks
        /// </summary>
        Task<Result<(int currentStreak, int longestStreak)>> CalculateAndUpdateStreaksAsync(Guid userId);

        /// <summary>
        /// Gets the user's current streak status
        /// </summary>
        Task<Result<(int currentStreak, int longestStreak, bool hasActivityToday)>> GetStreakStatusAsync(Guid userId);
    }