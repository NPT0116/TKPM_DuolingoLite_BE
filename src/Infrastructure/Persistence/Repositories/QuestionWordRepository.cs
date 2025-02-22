using System;
using Domain.Entities.Learning.Questions.QuestionOptions;
using Domain.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class QuestionWordRepository : IQuestionWordRepository
{
    private readonly ApplicationDbContext _context;
    public QuestionWordRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<QuestionWord>> GetQuestionWordsByQuestionIdAsync(Guid questionId)
    {   
        var questionWords = await _context.QuestionWords
            .Where(qw => qw.Question.Id == questionId).ToListAsync();
        return questionWords;
    }
}
