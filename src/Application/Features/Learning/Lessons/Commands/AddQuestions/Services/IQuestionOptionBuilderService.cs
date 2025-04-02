using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Learning.Questions.Enums;
using Domain.Entities.Learning.Questions.QuestionOptions;
using SharedKernel;
using LearningQuestion = Domain.Entities.Learning.Questions.Question;
namespace Application.Features.Learning.Lessons.Commands.AddQuestions.Services
{
    public interface IQuestionOptionBuilderService
    {
        Task<Result<List<QuestionOptionBase>>> BuildQuestionOptions(List<OptionBaseDto> options, LearningQuestion question, QuestionType type);
    }
}