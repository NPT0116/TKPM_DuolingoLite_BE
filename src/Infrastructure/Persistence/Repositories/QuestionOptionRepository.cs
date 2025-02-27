using System;
using Domain.Entities.Learning.Questions.QuestionOptions;
using Domain.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class QuestionOptionRepository : IQuestionOptionRepository
{
    private readonly ApplicationDbContext _context;
    public QuestionOptionRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<List<QuestionOptionBase>> GetQuestionOptionsByQuestionIdAsync(Guid questionId)
    {
        return await _context.QuestionOptions.Include(qo => qo.Option)
        .Include(qo => qo.Question).ThenInclude(q => q.Image)
        .Include(qo => qo.Question).ThenInclude(q => q.Audio)
        .Where(qo => qo.Question.Id == questionId).ToListAsync();
    }
}
