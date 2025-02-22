using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Seed.JsonModels
{
    public class OptionJsonModel
    {
        public string VietnameseText { get; set; }
        public string EnglishText { get; set; }
        public string Image { get; set; }
        public string Audio { get; set; }
        public bool IsReused { get; set; }
    }

    public class BuildSentenceOptionJsonModel : OptionJsonModel
    {
        public int Order { get; set; }
    }

    public class MultipleChoiceOptionJsonModel : OptionJsonModel
    {
        public bool IsCorrect { get; set; }
    }

    public class MatchingOptionJsonModel : OptionJsonModel {}

    public class PronunciationOptionJsonModel : OptionJsonModel {}
}