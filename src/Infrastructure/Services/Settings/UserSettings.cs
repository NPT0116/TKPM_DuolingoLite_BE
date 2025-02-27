using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Services.Settings
{
    public class UserSettings
    {
        public int NumberOfUsers { get; set; }
        public int NumberOfDays { get; set; }
        public string Password { get; set; }
    }
}