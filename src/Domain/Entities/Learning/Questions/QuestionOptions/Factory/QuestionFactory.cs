using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Learning.Questions.Configurations;
using Domain.Entities.Learning.Questions.Enums;
using SharedKernel;

namespace Domain.Entities.Learning.Questions.QuestionOptions.Factory
{
    public class QuestionFactory : IQuestionFactory
    {
        public Result<Question> Create(
            string? instruction, string? vietnameseText, Media.Media? audio, 
            string? englishText, Media.Media? image, QuestionType type, 
            Configuration questionConfiguration, Configuration optionConfiguration, int order)
        {
            var imageUrl = image?.FileName;
            if (new[]
            {
                (instruction, questionConfiguration.Instruction),
                (vietnameseText, questionConfiguration.VietnameseText),
                (englishText, questionConfiguration.EnglishText),
                (imageUrl, questionConfiguration.Image),
            }.Any(pair => (pair.Item1 == null && pair.Item2 == true) || (pair.Item1 != null && pair.Item2 == false)))
            {
                return Result.Failure<Question>(QuestionError.InvalidQuestionConfiguration);
            }

            if(type != QuestionType.Pronunciation)
            {
                if(optionConfiguration.Audio == false
                   && optionConfiguration.EnglishText == false
                   && optionConfiguration.VietnameseText == false
                   && optionConfiguration.Instruction == false
                   && optionConfiguration.Image == false)
                {
                    return Result.Failure<Question>(QuestionError.InvalidOptionConfiguration);
                }
            }

            var createQuestion = Domain.Entities.Learning.Questions.Question.Create(
                instruction,
                vietnameseText,
                audio,
                englishText,
                image,
                type,
                questionConfiguration,
                optionConfiguration,
                order
            );

            return createQuestion;
        }
    }
}