using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Learning.Questions.Enums;
using Domain.Entities.Learning.Questions.Options;
using Domain.Entities.Learning.Words.Enums;
using SharedKernel;

namespace Domain.Entities.Learning.Questions.QuestionOptions.Factory
{
    public class QuestionOptionFactory : IQuestionOptionFactory
    {
        public Result<QuestionOptionBase> Create(QuestionType type, Question question, Option option, int order, bool? isCorrect, MatchingQuestionOptionType? sourceType, MatchingQuestionOptionType? targetType, int? position)
        {
            return type switch
            {
                QuestionType.MultipleChoice => CreateMultipleChoice(question, option, order, isCorrect),
                QuestionType.Matching => CreateMatching(question, option, order, sourceType, targetType),
                QuestionType.BuildSentence => CreateBuildSentence(question, option, order, position),
                _ => Result.Failure<QuestionOptionBase>(QuestionOptionError.QuestionTypeNotSupported)
            };
        }

        private Result<QuestionOptionBase> CreateMultipleChoice(Question question, Option option, int order, bool? isCorrect)
        {
            if(isCorrect == null) return Result.Failure<QuestionOptionBase>(QuestionOptionError.IsCorrectFieldMissing);
            var create = MultipleChoiceQuestionOption.Create(question, option, (bool)isCorrect, order);
            
            if(create.IsFailure) return Result.Failure<QuestionOptionBase>(create.Error);
            return Result.Success<QuestionOptionBase>(create.Value);
        }

        private Result<QuestionOptionBase> CreateMatching(Question question, Option option, int order, MatchingQuestionOptionType? sourceType, MatchingQuestionOptionType? targetType)
        {
            if(sourceType == null) return Result.Failure<QuestionOptionBase>(QuestionOptionError.SourceTypeFieldMissing);
            if(targetType == null) return Result.Failure<QuestionOptionBase>(QuestionOptionError.TargetTypeFieldMissing);
            if(targetType == sourceType) return Result.Failure<QuestionOptionBase>(QuestionOptionError.TargetTypeMustBeDifferentFromSourceType);

            var createQuestionOption = MatchingQuestionOption.Create(question, option, sourceType.Value, targetType.Value, order);
            if(createQuestionOption.IsFailure) return Result.Failure<QuestionOptionBase>(createQuestionOption.Error);
            return Result.Success<QuestionOptionBase>(createQuestionOption.Value);
        }

        private Result<QuestionOptionBase> CreateBuildSentence(Question question, Option option, int order, int? position)
        {
            if(position == null) return Result.Failure<QuestionOptionBase>(QuestionOptionError.NoPositionSpecified);
            var sourceLanguage = string.IsNullOrEmpty(question.EnglishText) ? Language.vi : Language.en;
            
            if(sourceLanguage == Language.en && option.VietnameseText == null) 
                return Result.Failure<QuestionOptionBase>(QuestionOptionError.MissingTextForBuildSentenceOption(Language.vi));

            if(sourceLanguage == Language.vi && option.EnglishText == null) 
                return Result.Failure<QuestionOptionBase>(QuestionOptionError.MissingTextForBuildSentenceOption(Language.en));
            var createQuestionOption = BuildSentenceQuestionOption.Create(question, option, (int)position, order);
            if(createQuestionOption.IsFailure) return Result.Failure<QuestionOptionBase>(createQuestionOption.Error);
            return Result.Success<QuestionOptionBase>(createQuestionOption.Value);
        }
    }
}