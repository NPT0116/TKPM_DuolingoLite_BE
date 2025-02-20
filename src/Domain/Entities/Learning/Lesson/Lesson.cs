using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Domain.Entities.Question;
using SharedKernel;

namespace Domain.Entities.Course
{
    public class Lesson : Entity
    {
        public string Title { get; private set; } = string.Empty;
        public int XpEarned { get; private set; }
        public int Order { get; private set; }
        public readonly List<Question.Question> _questions = new();
        public IReadOnlyList<Question.Question> Questions => _questions.AsReadOnly();
        

        private Lesson() { }

        public Lesson(string title, int xpEarned, int order)
        {
            Title = title;
            XpEarned = xpEarned;
            Order = order;
        }

        public static Result<Lesson> Create(
            string title, 
            int xpEarned, 
            int order)
        {
            if(string.IsNullOrEmpty(title))
            {
                return Result.Failure<Lesson>(LessonError.TitleIsRequired());
            }

            if(xpEarned < 0)
            {
                return Result.Failure<Lesson>(LessonError.XpEarnedMustBeGreaterThanZero());
            }

            if(order < 0)
            {
                return Result.Failure<Lesson>(LessonError.OrderMustBeGreaterThanZero());
            }

            return Result.Success(new Lesson(title, xpEarned, order));

        }
    }
}