using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Users;

namespace Domain.Repositories
{
    public interface IUserRepository
    {
        Task<UserProfile?> GetUserProfileById(Guid userId);
        Task<UserProfile?> CreateUserProfile(UserProfile userProfile);
        Task<UserActivity?> CreateUserActivity(UserActivity userActivity);
        Task<UserStats?> CreateUserStats(UserStats userStats);
        Task<UserStats?> GetUserStatsById(Guid userId);
        Task<List<UserActivity>> GetUserActivitiesByUserId(Guid userId);
        Task<List<UserActivity>> GetUserActivitiesWithinDateRangeByUserId(Guid userId, DateTime startDate, DateTime endDate);
        Task<int> GetTotalUsersCount();

        Task<UserStats?> UpdateUserStats(UserStats userStats);

    }
}