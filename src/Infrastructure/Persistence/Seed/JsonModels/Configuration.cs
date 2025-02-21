using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Seed.JsonModels
{
    public class Configuration
    {
        public bool Audio { get; set; }
        public bool EnglishText { get; set; }
        public bool VietnameseText { get; set; }
        public bool Instruction { get; set; }
        public bool Image { get; set; }
    }
}