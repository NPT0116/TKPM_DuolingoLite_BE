using Domain.Entities.Learning.Questions.Options;
using SharedKernel;

namespace Domain.Entities.Learning.Questions.QuestionOptions
{
    public class MultipleChoiceQuestionOption : QuestionOptionBase
    {
        public bool IsCorrect { get; private set; }

        private MultipleChoiceQuestionOption() { } // For EF Core

        private MultipleChoiceQuestionOption(Question question, Option option, bool isCorrect, int order) 
            : base(question, option, order)
        {
            IsCorrect = isCorrect;
        }

        public static Result<MultipleChoiceQuestionOption> Create(Question question, Option option, bool isCorrect, int order)
        {
            return Result.Success(new MultipleChoiceQuestionOption(question, option, isCorrect, order));
        }
    }
} 