using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Question;
using SharedKernel;

namespace Domain.Entities.Learning.Question.QuestionOption
{
    public class QuestionWord : Entity
    {
        public Word.Word Word { get; private set; }
        public Domain.Entities.Question.Question Question { get; private set; }
        public int Order { get; private set; }

        private QuestionWord() { } // For EF Core

        private QuestionWord(Word.Word word, Domain.Entities.Question.Question question, int order)
        {
            Word = word;
            Question = question;
            Order = order;
        }

        public static Result<QuestionWord> Create(Word.Word word, Domain.Entities.Question.Question question, int order)
        {
            return Result.Success(new QuestionWord(word, question, order));
        }
    }
}