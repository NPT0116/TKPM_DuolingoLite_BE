using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Users.Constants;
using SharedKernel;

namespace Domain.Entities.Users
{
    public class UserStats : Entity
    {
        public Guid UserId { get; private set; }
        public int ExperiencePoint { get; private set; }
        public int Heart { get; private set; }
        public int CurrentStreak { get; private set; }
        public int LongestStreak { get; private set; }
        public DateTime? LastActiveDate { get; private set; }

        private UserStats() { }

        public UserStats(Guid userId)
        {
            UserId = userId;
            ExperiencePoint = 0;
            Heart = 5;
            CurrentStreak = 0;
            LongestStreak = 0;
            LastActiveDate = DateTime.UtcNow;
        }

        public void EarnExperience(int points) => ExperiencePoint += points;
        public void LoseHeart() => Heart = Math.Max(Heart - 1, HeartConstants.MINIMUM_HEART);
        public void GainHeart() => Heart = Math.Min(Heart + 1, HeartConstants.MAXIMUM_HEART);

        public static Result<UserStats> Create(Guid userId)
        {
            var userStats = new UserStats(userId);
            return Result.Success(userStats);
        }
    }

}