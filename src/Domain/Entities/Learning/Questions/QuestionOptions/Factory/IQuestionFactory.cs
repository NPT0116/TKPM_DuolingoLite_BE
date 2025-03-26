using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Learning.Questions.Configurations;
using Domain.Entities.Learning.Questions.Enums;
using Domain.Entities.Media;
using SharedKernel;

namespace Domain.Entities.Learning.Questions.QuestionOptions.Factory
{
    public interface IQuestionFactory
    {
        Result<Question> Create(
            string? instruction,
            string? vietnameseText,
            Media.Media? audio,
            string? englishText,
            Media.Media? image,
            QuestionType type,
            Configuration questionConfiguration,
            Configuration optionConfiguration,
            int order
        );
    }
}