using System;
using Domain.Entities.Learning.Questions;

namespace Domain.Repositories;

public interface IQuestionRepository
{
    Task<Question?> GetQuestionByIdAsync(Guid id);

}
