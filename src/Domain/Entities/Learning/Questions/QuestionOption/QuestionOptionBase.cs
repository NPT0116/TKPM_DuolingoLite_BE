using Domain.Entities.Option;
using SharedKernel;

namespace Domain.Entities.Question.QuestionOption
{
    public abstract class QuestionOptionBase : Entity
    {
        public int Order { get; private set; }

        public Question Question { get; private set; }
        public Option.Option Option { get; private set; }

        protected QuestionOptionBase(Question question, Option.Option option, int order)
        {
            Question = question;
            Option = option;
            Order = order;
        }

        protected QuestionOptionBase() { } // For EF Core
    }
} 