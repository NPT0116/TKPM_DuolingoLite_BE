using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Learning.Questions.Configurations;
using Domain.Entities.Learning.Questions.Enums;
using SharedKernel;
using LearningQuestion = Domain.Entities.Learning.Questions.Question;
namespace Application.Features.Learning.Lessons.Commands.AddQuestions.Services
{
    public interface IQuestionBuilderService
    {
        Task<Result<LearningQuestion>> BuildQuestion(
            string? instruction,
            string? vietnameseText,
            string? englishText,
            string? image,
            string? audio,
            QuestionType type,
            ConfigurationDto questionConfiguration,
            ConfigurationDto optionConfiguration,
            int order
        );
    }
}