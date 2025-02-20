using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Learning.Words;
using SharedKernel;

namespace Domain.Entities.Learning.Questions.QuestionOptions
{
    public class QuestionWord : Entity
    {
        public Word Word { get; private set; }
        public Question Question { get; private set; }
        public int Order { get; private set; }

        private QuestionWord() { } // For EF Core

        private QuestionWord(Word word, Question question, int order)
        {
            Word = word;
            Question = question;
            Order = order;
        }

        public static Result<QuestionWord> Create(Word word, Question question, int order)
        {
            return Result.Success(new QuestionWord(word, question, order));
        }
    }
}