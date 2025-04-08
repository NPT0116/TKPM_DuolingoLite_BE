using Domain.Entities.Learning.Questions;
using Microsoft.VisualBasic;
using SharedKernel;

namespace Domain.Entities.Learning.Lessons
{
    public class Lesson : Entity
    {
        public string Title { get; private set; } = string.Empty;
        public int XpEarned { get; private set; }
        public int Order { get; private set; }
        public readonly List<Question> _questions = new();
        public IReadOnlyList<Question> Questions => _questions.AsReadOnly();
        

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

        public void AddQuestion(Question question)
        {
            _questions.Add(question);
        }

        public Result SetTitle(string title)
        {
            if(string.IsNullOrEmpty(title))
            {
                return Result.Failure(LessonError.TitleIsRequired());
            }

            Title = title;
            return Result.Success();
        }

        public Result SetXpEarned(int xpEarned)
        {
            if(xpEarned < 0)
            {
                return Result.Failure(LessonError.XpEarnedMustBeGreaterThanZero());
            }

            XpEarned = xpEarned;
            return Result.Success();
        }

        public void SetOrder(int order)
        {
            Order = order;
        }

        public Result RemoveQuestion(int questionOrder)
        {
            if(questionOrder < 1 || questionOrder > _questions.Count)
            {
                return Result.Failure(LessonError.QuestionOrderNotFound(questionOrder, Id));
            }

            _questions.RemoveAt(questionOrder - 1);
            
            for(var i = 0; i < _questions.Count; i++)
            {
                _questions[i].SetOrder(i + 1);
            }

            return Result.Success();
        }

        public Result<Question> GetQuestionByOrder(int questionOrder)
        {
            if(questionOrder < 1 || questionOrder > _questions.Count)
            {
                return Result.Failure<Question>(LessonError.QuestionOrderNotFound(questionOrder, Id));
            }

            return Result.Success(_questions[questionOrder - 1]);
        } 
    }
}