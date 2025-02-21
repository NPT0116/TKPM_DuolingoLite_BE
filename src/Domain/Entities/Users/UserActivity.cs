using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharedKernel;

namespace Domain.Entities.Users
{
    public class UserActivity : Entity
    {
        public Guid UserId { get; private set; }
        public DateTime Date { get; private set; }
        public bool IsActive { get; private set; }

        private UserActivity() { }

        private UserActivity(Guid userId, DateTime date, bool isActive)
        {
            UserId = userId;
            Date = date;
            IsActive = isActive;
        }
    }

}