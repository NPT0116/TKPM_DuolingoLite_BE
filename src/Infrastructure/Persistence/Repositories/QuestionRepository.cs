using System;
using Domain.Entities.Learning.Questions;
using Domain.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class QuestionRepository : IQuestionRepository
{
    private readonly ApplicationDbContext _context;
    public QuestionRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<Question?> GetQuestionByIdAsync(Guid id)
    {
        return await _context.Questions.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<Question>> GetQuestionsThatUseOption(Guid optionId)
    {
        return await _context.Questions
            .Include(q => q.Options)
                .ThenInclude(qo => qo.Option)
            .Include(q => q.Image)
            .Include(q => q.Audio)
            .Where(q => q.Options.Any(o => o.Id == optionId))
            .ToListAsync();
    }   
}
