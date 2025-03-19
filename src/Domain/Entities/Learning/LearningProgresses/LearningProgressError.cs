using System;
using SharedKernel;

namespace Domain.Entities.Learning.LearningProgresses;

public static class LearningProgressError
{
    public static Error LearningProgressNotFound(Guid id) => Error.NotFound("LearningProgress.LearningProgressNotFound", $"LearningProgress with id {id} not found");
    public static Error LearningProgresssForUserNotFound(Guid userId) => Error.NotFound("LearningProgress.LearningProgresssForUserNotFound", $"LearningProgresss for user with id {userId} not found");
    public static Error LessonOrderMismatch(int lessonOrder, int learningProgressLessonOrder) => Error.Validation("LearningProgress.LessonOrderMismatch", $"Lesson order mismatch. Lesson order is {lessonOrder} but learning progress lesson order is {learningProgressLessonOrder}");
    public static Error LearningProgressAlreadyCompleted(Guid id) => Error.Validation("LearningProgress.LearningProgressAlreadyCompleted", $"LearningProgress with id {id} is already completed");
    public static Error UserAlreadyEnrolledInCourse() => Error.Validation("LearningProgress.UserAlreadyEnrolledToCourse", "User is already enrolled to the course");
}
