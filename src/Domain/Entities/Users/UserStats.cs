using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharedKernel;

namespace Domain.Entities.Users
{
    public class UserStats : Entity
    {
        public int ExperiencePoint { get; private set; }
        public int Heart { get; private set; }
        public int CurrentStreak { get; private set; }
        public int LongestStreak { get; private set; }
        public DateTime? LastActiveDate { get; private set; }

        private UserStats() { }

        public UserStats(string userId)
        {
            ExperiencePoint = 0;
            Heart = 5;
            CurrentStreak = 0;
            LongestStreak = 0;
            LastActiveDate = DateTime.UtcNow;
        }

        public void EarnExperience(int points) => ExperiencePoint += points;
        public void LoseHeart() => Heart = Math.Max(Heart - 1, 0);
    }

}