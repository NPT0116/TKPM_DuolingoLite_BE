using Domain.Entities.Learning.Questions.Options;
using SharedKernel;

namespace Domain.Entities.Learning.Questions.QuestionOptions
{
    public class BuildSentenceQuestionOption : QuestionOptionBase
    {
        public int Position { get; private set; }

        private BuildSentenceQuestionOption() { } // For EF Core

        private BuildSentenceQuestionOption(Question question, Option option, int position, int order) 
            : base(question, option, order)
        {
            Position = position;
        }

        public static Result<BuildSentenceQuestionOption> Create(Question question, Option option, int position, int order)
        {
            return Result.Success(new BuildSentenceQuestionOption(question, option, position, order));
        }
    }
} 