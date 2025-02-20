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
    }
}