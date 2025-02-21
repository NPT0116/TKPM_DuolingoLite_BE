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
        return await _applicationDbContext.Lessons.FirstOrDefaultAsync(x => x.Id == id);
    }
}
