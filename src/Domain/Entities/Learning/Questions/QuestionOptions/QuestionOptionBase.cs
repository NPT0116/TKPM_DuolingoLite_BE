using Domain.Entities.Learning.Questions.Options;
using SharedKernel;

namespace Domain.Entities.Learning.Questions.QuestionOptions
{
    public abstract class QuestionOptionBase : Entity
    {
        public int Order { get; private set; }

        public Question Question { get; private set; }
        public Option Option { get; private set; }

        protected QuestionOptionBase(Question question, Option option, int order)
        {
            Question = question;
            Option = option;
            Order = order;
        }

        protected QuestionOptionBase() { } // For EF Core
    }
} 