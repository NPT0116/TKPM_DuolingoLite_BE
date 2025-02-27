using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Services.Settings
{
    public class AwsSettings
    {
        public string Profile { get; set; }
        public string Region { get; set; }
        public string BucketName { get; set; }
    }
}