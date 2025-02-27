using System;
using Application.Interface;
using Domain.Entities.Learning.Lessons;
using Domain.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class LessonRepository : ILessonRepository
{
    private readonly ApplicationDbContext _applicationDbContext;
    public LessonRepository(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }
public async Task<Lesson?> GetLessonByIdAsync(Guid id)
{
    return await _applicationDbContext.Lessons
        .AsSplitQuery() // Tách query, tối ưu khi có nhiều Include
        .Include(l => l.Questions)
            .ThenInclude(q => q.Options)
            .ThenInclude(o => o.Option)
            .ThenInclude(o => o.Image)
        .Include(l => l.Questions)
            .ThenInclude(q => q.Options)
            .ThenInclude(o => o.Option)
            .ThenInclude(o => o.Audio)   

        .Include(l => l.Questions)
            .ThenInclude(q => q.Image)
        // Include Audio
        .Include(l => l.Questions)
            .ThenInclude(q => q.Audio)
        // Include QuestionConfiguration
        .Include(l => l.Questions)
            .ThenInclude(q => q.QuestionConfiguration)
        // Include OptionConfiguration
        .Include(l => l.Questions)
            .ThenInclude(q => q.OptionConfiguration)
        // Include QuestionWord
        .Include(l => l.Questions)
            .ThenInclude(q => q.Words)
            .ThenInclude(w => w.Word)
            .ThenInclude(w => w.Audio)
        .FirstOrDefaultAsync(x => x.Id == id);
}

}
