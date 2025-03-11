using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharedKernel.Cache
{
    public static class Cache
    {
        private const string HeartPrefix = "user:heart:";
        public static string GetUserHeartKey(Guid userId) => $"{HeartPrefix}{userId}";
    }
}