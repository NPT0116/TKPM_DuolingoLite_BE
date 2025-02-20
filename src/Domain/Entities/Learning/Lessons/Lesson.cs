using Domain.Entities.Learning.Questions;
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
    }
}