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
        public async Task<UserProfile?> GetUserProfileById(Guid userId)
        {
            return await _context.UserProfiles
                .Include(up => up.ProfileImage)
                .Include(up => up.Subscription)
                .FirstOrDefaultAsync(up => EF.Property<Guid>(up, "UserId") == userId);
        }

    }
}