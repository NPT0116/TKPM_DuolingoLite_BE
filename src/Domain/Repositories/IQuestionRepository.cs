using System;
using Domain.Entities.Learning.Questions;

namespace Domain.Repositories;

public interface IQuestionRepository
{
    Task<Question> GetQuestionByIdAsync(Guid id);
    Task<IEnumerable<Question>> GetQuestionsAsync();
    Task AddQuestionAsync(Question question);
    Task UpdateQuestionAsync(Question question);
    Task DeleteQuestionAsync(Guid id);
}
