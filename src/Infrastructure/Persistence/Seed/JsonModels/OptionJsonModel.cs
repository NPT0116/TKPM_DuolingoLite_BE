using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Seed.JsonModels
{
    public class OptionJsonModel
    {
        public string Text { get; set; }
        public bool IsCorrect { get; set; }
        public int Order { get; set; }
    }

}