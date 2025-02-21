using System;
using Domain.Entities.Learning.Lessons;

namespace Domain.Repositories;

public interface ILessonRepository
{
    Task<Lesson> GetLessonByIdAsync(Guid id);
    Task<IEnumerable<Lesson>> GetLessonsAsync();
    Task AddLessonAsync(Lesson lesson);
    Task UpdateLessonAsync(Lesson lesson);
    Task DeleteLessonAsync(Guid id);
}
