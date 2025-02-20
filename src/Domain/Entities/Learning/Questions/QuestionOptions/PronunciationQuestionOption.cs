using Domain.Entities.Learning.Questions.Options;
using SharedKernel;

namespace Domain.Entities.Learning.Questions.QuestionOptions
{
    public class PronunciationQuestionOption : QuestionOptionBase
    {
        private PronunciationQuestionOption() { } // For EF Core

        private PronunciationQuestionOption(Question question, Option option, int order) 
            : base(question, option, order)
        {
        }
        
        public static Result<PronunciationQuestionOption> Create(Question question, Option option, int order)
        {
            return Result.Success(new PronunciationQuestionOption(question, option, order));
        }
    }
} 