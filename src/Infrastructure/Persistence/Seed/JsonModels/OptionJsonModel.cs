using JsonSubTypes;
using Newtonsoft.Json;

namespace Infrastructure.Persistence.Seed.JsonModels
{
    [JsonConverter(typeof(JsonSubtypes), "Type")]
    [JsonSubtypes.KnownSubType(typeof(MatchingOptionJsonModel), "Matching")]
    [JsonSubtypes.KnownSubType(typeof(MultipleChoiceOptionJsonModel), "MultipleChoice")]
    [JsonSubtypes.KnownSubType(typeof(BuildSentenceOptionJsonModel), "BuildSentence")]
    [JsonSubtypes.KnownSubType(typeof(PronunciationOptionJsonModel), "Pronunciation")]
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

    public class MatchingOptionJsonModel : OptionJsonModel 
    {
        public string TargetType { get; set; }
        public string SourceType { get; set; }
    }

    public class PronunciationOptionJsonModel : OptionJsonModel {}
}