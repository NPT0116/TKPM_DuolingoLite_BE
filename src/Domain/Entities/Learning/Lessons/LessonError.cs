using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharedKernel;

namespace Domain.Entities.Learning.Lessons
{
    public class LessonError
    {
        public static Error LessonNotFound => Error.NotFound("Lesson.NotFound", "Lesson not found");
        public static Error TitleIsRequired() => Error.Validation("Lesson.TitleIsRequired", "Title is required");
        public static Error XpEarnedMustBeGreaterThanZero() => Error.Validation("Lesson.XpEarnedMustBeGreaterThanZero", "Xp earned must be greater than zero");
        public static Error OrderMustBeGreaterThanZero() => Error.Validation("Lesson.OrderMustBeGreaterThanZero", "Order must be greater than zero");
    }
}