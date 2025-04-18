using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Learning.Questions;
using Domain.Entities.Learning.Questions.QuestionOptions;
using SharedKernel;

namespace Application.Features.Learning.Lessons.Commands.AddQuestions.Services
{
    public interface IWordGeneratorService
    {
        Task<Result<List<QuestionWord>>> GenerateWords(Domain.Entities.Learning.Questions.Question question, string englishText);
    }
}