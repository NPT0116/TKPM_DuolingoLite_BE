using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Services.Settings
{
    public class BackgroundSettings
    {
        public string HeartSyncInterval { get; set; }
        public string HeartRecoveryInterval { get; set; }
    }
}