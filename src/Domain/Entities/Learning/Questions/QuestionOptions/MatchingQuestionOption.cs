using Domain.Entities.Learning.Questions.Options;
using SharedKernel;

namespace Domain.Entities.Learning.Questions.QuestionOptions
{
    public class MatchingQuestionOption : QuestionOptionBase
    {
        public MatchingQuestionOptionType SourceType { get; private set; }
        public MatchingQuestionOptionType TargetType { get; private set; }

        private MatchingQuestionOption() { } // For EF Core

        private MatchingQuestionOption(Question question, Option option, MatchingQuestionOptionType sourceType, MatchingQuestionOptionType targetType, int order) 
            : base(question, option, order)
        {
            SourceType = sourceType;
            TargetType = targetType;
        }

        public static Result<MatchingQuestionOption> Create(Question question, Option option, MatchingQuestionOptionType sourceType, MatchingQuestionOptionType targetType, int order)
        {
            return Result.Success(new MatchingQuestionOption(question, option, sourceType, targetType, order));
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