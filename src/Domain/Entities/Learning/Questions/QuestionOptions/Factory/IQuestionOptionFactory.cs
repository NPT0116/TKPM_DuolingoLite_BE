using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Learning.Questions.Enums;
using Domain.Entities.Learning.Questions.Options;
using SharedKernel;

namespace Domain.Entities.Learning.Questions.QuestionOptions.Factory
{
    public interface IQuestionOptionFactory
    {
        Result<QuestionOptionBase> Create(
            QuestionType type,
            Question question,
            Option option,
            int order,
            bool? isCorrect,
            MatchingQuestionOptionType? sourceType,
            MatchingQuestionOptionType? targetType,
            int? position
        );
    }
}