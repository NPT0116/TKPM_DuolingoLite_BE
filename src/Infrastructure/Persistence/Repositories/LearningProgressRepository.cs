using System;
using Domain.Entities.Learning.LearningProgresses;
using Domain.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class LearningProgressRepository : ILearningProgressRepository
{
    private readonly ApplicationDbContext _context;
    public LearningProgressRepository(ApplicationDbContext context)
    {
        _context  = context;
    }

    public async Task<LearningProgress> AddAsync(LearningProgress learningProgress)
    {
         await _context.LearningProgresses.AddAsync(learningProgress);
        return learningProgress;
    }

    public async Task<LearningProgress?> GetLearningProgressByIdAsync(Guid id)
    {
       var lp =  await _context.LearningProgresses.FirstOrDefaultAsync(x => x.Id == id);
    return lp;
    }

    public Task<LearningProgress> GetLearningProgressByUserIdAndCourseIdAsync(Guid userId, Guid courseId)
    {
        var learningProgress = _context.LearningProgresses.FirstOrDefaultAsync(x => x.UserId == userId && x.Course.Id == courseId);
        return learningProgress;
    }

public async Task<LearningProgress?> GetLearningProgressByUserIdAsync(Guid userId)
{
    var lp = await _context.LearningProgresses
        .Include(lp => lp.Course)
        .FirstOrDefaultAsync(x => x.UserId == userId);

    return lp;
}

    public async Task<int> GetUserCountRegisteringCourse(Guid courseId)
    {
        return await _context.LearningProgresses.CountAsync(x => x.Course.Id == courseId);
    }

    public Task UpdateAsync(LearningProgress learningProgress)
    {
        _context.LearningProgresses.Update(learningProgress);
        return Task.CompletedTask;
    }
}
