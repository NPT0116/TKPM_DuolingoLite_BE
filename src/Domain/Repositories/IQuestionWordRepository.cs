using System;
using Domain.Entities.Learning.Questions.QuestionOptions;

namespace Domain.Repositories;

public interface IQuestionWordRepository
{
    Task<IEnumerable<QuestionWord>> GetQuestionWordsByQuestionIdAsync(Guid questionId);
}
