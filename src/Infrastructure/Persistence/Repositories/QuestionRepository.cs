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
    public Task<Question?> GetQuestionByIdAsync(Guid id)
    {
        return _context.Questions.FirstOrDefaultAsync(x => x.Id == id);
    }
}
