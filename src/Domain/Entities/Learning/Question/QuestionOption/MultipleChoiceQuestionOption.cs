using SharedKernel;

namespace Domain.Entities.Question.QuestionOption
{
    public class MultipleChoiceQuestionOption : QuestionOptionBase
    {
        public bool IsCorrect { get; private set; }

        private MultipleChoiceQuestionOption() { } // For EF Core

        private MultipleChoiceQuestionOption(Question question, Option.Option option, bool isCorrect, int order) 
            : base(question, option, order)
        {
            IsCorrect = isCorrect;
        }

        public static Result<MultipleChoiceQuestionOption> Create(Question question, Option.Option option, bool isCorrect, int order)
        {
            return Result.Success(new MultipleChoiceQuestionOption(question, option, isCorrect, order));
        }
    }
} 