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
        public static Error LessonHasBeenLearnedByUser(Guid courseId, int lessonOrder) => Error.Conflict(
            "Lesson.LessonHasBeenLearnedByUser",
            $"Can not delete lesson {lessonOrder} in course {courseId} because it has already been learned by a user"
        );
        public static Error QuestionOrderNotFound(int order, Guid lessonId) => Error.NotFound(
            "Lesson.QuestionOrderNotFound",
            $"Question with order {order} not found in lesson {lessonId}"
        );
        public static Error QuestionHasBeenUsedInSpacedRepetition(int order, Guid lessonId) => Error.Conflict(
            "Lesson.QuestionHasBeenUsedInSpacedRepetition",
            $"Question with order {order} in lesson {lessonId} has been used in spaced repetition"
        );
    }
}