using System;
using Domain.Entities.Learning.Lessons;

namespace Domain.Repositories;

public interface ILessonRepository
{
    Task<Lesson?> GetLessonByIdAsync(Guid id);

}
