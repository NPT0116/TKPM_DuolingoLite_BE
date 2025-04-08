using System;
using Domain.Entities.Learning.LearningProgresses;

namespace Domain.Repositories;

public interface ILearningProgressRepository
{
    Task<LearningProgress?> GetLearningProgressByIdAsync(Guid id);
    Task<LearningProgress?> GetLearningProgressByUserIdAsync(Guid UserId);
    // Task<IEnumerable<LearningProgress>> GetLearningProgressesAsync();
    // Task<LearningProgress> AddLearningProgressAsync(LearningProgress learningProgress);
    // Task<LearningProgress> UpdateLearningProgressAsync(LearningProgress learningProgress);
    // Task DeleteLearningProgressAsync(Guid id);
    Task UpdateAsync(LearningProgress learningProgress);
    Task <LearningProgress> GetLearningProgressByUserIdAndCourseIdAsync(Guid userId, Guid courseId);
    Task<LearningProgress> AddAsync(LearningProgress learningProgress);
    Task<int> GetUserCountRegisteringCourse(Guid courseId);
    Task<int> GetEnrolledUserCountForLessonAsync(Guid courseId, int lessonOrder);

}
