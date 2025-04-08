using System;
using Domain.Entities.Learning.Questions.QuestionOptions;

namespace Domain.Repositories;

public interface IQuestionOptionRepository
{
    Task<List<QuestionOptionBase>> GetQuestionOptionsByQuestionIdAsync(Guid questionId);
    Task<int> GetQuestionsCountByOptionAsync(Guid optionId);
}
