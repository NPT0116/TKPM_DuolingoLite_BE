using SharedKernel;

namespace Domain.Entities.Question.QuestionOption
{
    public class PronunciationQuestionOption : QuestionOptionBase
    {
        private PronunciationQuestionOption() { } // For EF Core

        private PronunciationQuestionOption(Question question, Option.Option option, int order) 
            : base(question, option, order)
        {
        }
        
        public static Result<PronunciationQuestionOption> Create(Question question, Option.Option option, int order)
        {
            return Result.Success(new PronunciationQuestionOption(question, option, order));
        }
    }
} 