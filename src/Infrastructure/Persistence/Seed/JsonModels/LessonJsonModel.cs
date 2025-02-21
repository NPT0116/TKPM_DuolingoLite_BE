using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Seed.JsonModels
{
    public class LessonJsonModel
    {
        public required string Title { get; set; }
        public int XpEarned { get; set; }
        public int Order { get; set; }
        public List<QuestionJsonModel> Questions { get; set; } = new();
    }

}