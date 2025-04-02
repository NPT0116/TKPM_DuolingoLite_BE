using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Users;
using Domain.Query.User;
using SharedKernel;

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
        // Task<User> GetUserById(Guid userId);
        Task<UserStats?> UpdateUserStats(UserStats userStats);

        Task<UserProfile?> UpdateUserProfile(UserProfile userProfile);
        Task<List<UserProfile>> GetExpiredSubscriptions();
        Task<PaginationResult<UserProfile>> GetAllUsers(GetAllUserQueryParams queryParams);
    }
}