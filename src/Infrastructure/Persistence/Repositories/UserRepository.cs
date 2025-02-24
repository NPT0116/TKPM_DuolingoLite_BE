using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Interface;
using Domain.Entities.Users;
using Domain.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
namespace Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
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
        
        
    }
}