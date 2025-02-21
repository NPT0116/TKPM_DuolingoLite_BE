using System;
using SharedKernel;

namespace Domain.Entities.Learning.LearningProgresses;

public static class LearningProgressError
{
    public static Error LearningProgressNotFound(Guid id) => Error.NotFound("LearningProgress.LearningProgressNotFound", $"LearningProgress with id {id} not found");
    public static Error LearningProgresssForUserNotFound(Guid userId) => Error.NotFound("LearningProgress.LearningProgresssForUserNotFound", $"LearningProgresss for user with id {userId} not found");
}
