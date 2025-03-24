using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Services.Settings
{
    public class BackgroundSettings
    {
        public string HeartSyncInterval { get; set; }
        public double HeartRecoveryInterval { get; set; }
        public string RefillHeartCheckInterval { get; set; }
    }
}