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
            var isConfigurationValid = IsConfigurationMatching(instruction, vietnameseText, audio, englishText, image, questionConfiguration);
            if (!isConfigurationValid) return Result.Failure<Question>(QuestionError.InvalidQuestionConfiguration);

            var isAnyConfigurationEnabled = IsAnyConfigurationEnabled(questionConfiguration);
            if (!isAnyConfigurationEnabled) return Result.Failure<Question>(QuestionError.AllPromptsNull()); 
            
            var validate = type switch
            {
                QuestionType.MultipleChoice => ValidateMultipleChoiceQuestion(questionConfiguration),
                QuestionType.Matching => ValidateMatchingQuestion(questionConfiguration),
                QuestionType.Pronunciation => ValidatePronunciationQuestion(questionConfiguration),
                QuestionType.BuildSentence => ValidateBuildSentenceQuestion(questionConfiguration),
                _ => Result.Failure<Question>(QuestionError.InvalidQuestionConfiguration)
            };

            if(validate.IsFailure) return Result.Failure<Question>(validate.Error);
            var createQuestion = Question.Create(instruction, vietnameseText, audio, englishText, image, type, questionConfiguration, optionConfiguration, order);
            return createQuestion;
        }

        private bool IsAnyConfigurationEnabled(Configuration configuration)
        {
            return configuration.Audio || configuration.EnglishText || 
                configuration.VietnameseText || configuration.Instruction || configuration.Image;
        }

        private bool IsConfigurationMatching(
            string? instruction, string? vietnameseText, Media.Media? audio, 
            string? englishText, Media.Media? image, Configuration questionConfiguration)
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
                return false;
            }
            return true;
        }

        private Result ValidateMultipleChoiceQuestion(Configuration questionConfiguration
        )
        {
            return Result.Success();
        }

        private Result ValidateMatchingQuestion(Configuration questionConfiguration)
        {
            return Result.Success();
        }

        private Result ValidatePronunciationQuestion(Configuration questionConfiguration)
        {
            return Result.Success();
        }

        private Result ValidateBuildSentenceQuestion(Configuration questionConfiguration)
        {
            if(!(questionConfiguration.VietnameseText ^ questionConfiguration.EnglishText))
            {
                return Result.Failure(QuestionError.EnglishOrVitenameseTextRequired);
            }
            return Result.Success();
        }
    }
}