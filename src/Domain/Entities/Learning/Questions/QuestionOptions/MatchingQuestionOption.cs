using Domain.Entities.Learning.Questions.Options;
using SharedKernel;

namespace Domain.Entities.Learning.Questions.QuestionOptions
{
    public class MatchingQuestionOption : QuestionOptionBase
    {
        public Guid MatchWithOptionId { get; private set; }
        public Option MatchWithOption { get; private set; }
        public MatchingQuestionOptionType SourceType { get; private set; }
        public MatchingQuestionOptionType TargetType { get; private set; }

        private MatchingQuestionOption() { } // For EF Core

        private MatchingQuestionOption(Question question, Option option, Option matchWithOption, MatchingQuestionOptionType sourceType, MatchingQuestionOptionType targetType, int order) 
            : base(question, option, order)
        {
            MatchWithOptionId = matchWithOption.Id;
            MatchWithOption = matchWithOption;
            SourceType = sourceType;
            TargetType = targetType;
        }

        public static Result<MatchingQuestionOption> Create(Question question, Option option, Option matchWithOption, MatchingQuestionOptionType sourceType, MatchingQuestionOptionType targetType, int order)
        {
            return Result.Success(new MatchingQuestionOption(question, option, matchWithOption, sourceType, targetType, order));
        }
    }

    public enum MatchingQuestionOptionType
    {
        Image,
        Audio,
        EnglishText,
        VietnameseText
    }
} 