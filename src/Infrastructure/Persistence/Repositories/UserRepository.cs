using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Interface;
using Domain.Entities.Users;
using Domain.Query.User;
using Domain.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
namespace Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IDateTimeProvider _dateTimeProvider;
        public UserRepository(ApplicationDbContext context, IDateTimeProvider dateTimeProvider)
        {
            _context = context;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<UserActivity?> CreateUserActivity(UserActivity userActivity)
        {
            await _context.UserActivities.AddAsync(userActivity);
            return userActivity;
        }

        public async Task<UserProfile?> CreateUserProfile(UserProfile userProfile)
        {
            await _context.UserProfiles.AddAsync(userProfile);
            return userProfile;
        }

        public async Task<UserStats?> CreateUserStats(UserStats userStats)
        {
            await _context.UserStats.AddAsync(userStats);
            return userStats;
        }

        public async Task<UserProfile?> GetUserProfileById(Guid userId)
        {
            return await _context.UserProfiles
                .Include(up => up.ProfileImage)
                .Include(up => up.Subscription)
                .FirstOrDefaultAsync(up => up.UserId == userId);
        }

        public async Task<UserStats?> GetUserStatsById(Guid userId)
        {
            return await _context.UserStats.FirstOrDefaultAsync(us => us.UserId == userId);
        }

        public async Task<List<UserActivity>> GetUserActivitiesByUserId(Guid userId)
        {
            return await _context.UserActivities.Where(ua => ua.UserId == userId).ToListAsync();
        }

        public async Task<List<UserActivity>> GetUserActivitiesWithinDateRangeByUserId(Guid userId, DateTime startDate, DateTime endDate)
        {
            return await _context.UserActivities.Where(ua => ua.UserId == userId && ua.Date >= startDate && ua.Date <= endDate).ToListAsync();
        }
        
        public async Task<int> GetTotalUsersCount()
        {
            return await _context.UserProfiles.CountAsync();
        }

        public async Task<UserStats?> UpdateUserStats(UserStats userStats)
        {
             _context.UserStats.Update(userStats);
            return userStats;
        }

        public async Task<UserProfile?> UpdateUserProfile(UserProfile userProfile)
        {
            _context.UserProfiles.Update(userProfile);
            return userProfile;
        }

        public async Task<List<UserProfile>> GetExpiredSubscriptions()
        {
            var now = _dateTimeProvider.UtcNow;

        var expiredUsers = await _context.UserProfiles
            .Include(u => u.Subscription)
            .Where(u => u.Subscription != null && u.Subscription.ExpiredDate < now)
            .ToListAsync();
        return expiredUsers;
        }

        public async Task<PaginationResult<UserProfile>> GetAllUsers(GetAllUserQueryParams queryParams)
        {
            var query = _context.UserProfiles
                .Include(up => up.ProfileImage)
                .Include(up => up.Subscription)
                .AsQueryable();

            if (!string.IsNullOrEmpty(queryParams.SearchKeyword))
            {
                query = query.Where(up => up.NickName.Contains(queryParams.SearchKeyword) || up.LastName.Contains(queryParams.SearchKeyword));
            }


            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / queryParams.PageSize);
            var items = await query
                .Skip((queryParams.PageNumber - 1) * queryParams.PageSize)
                .Take(queryParams.PageSize)
                .OrderBy(u => u.NickName)
                .ToListAsync();
            return new PaginationResult<UserProfile>(items, queryParams.PageNumber, queryParams.PageSize, totalCount);
        }
    }
}