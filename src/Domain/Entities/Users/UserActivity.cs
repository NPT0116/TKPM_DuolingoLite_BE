using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharedKernel;

namespace Domain.Entities.User
{
    public class UserActivity : Entity
    {
        public DateTime Date { get; private set; }
        public bool IsActive { get; private set; }

        private UserActivity() { }

        private UserActivity(DateTime date, bool isActive)
        {
            Date = date;
            IsActive = isActive;
        }
    }

}