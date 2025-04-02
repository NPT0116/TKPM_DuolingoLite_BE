using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Learning.Questions.Enums;
using Domain.Entities.Learning.Questions.Options;
using Domain.Entities.Learning.Questions.QuestionOptions;
using Domain.Entities.Learning.Questions.QuestionOptions.Factory;
using Domain.Entities.Learning.Questions.QuestionOptions.Validator;
using Domain.Repositories;
using SharedKernel;
using LearningQuestion = Domain.Entities.Learning.Questions.Question;
namespace Application.Features.Learning.Lessons.Commands.AddQuestions.Services
{
    public class QuestionOptionBuilderService : IQuestionOptionBuilderService
    {
        private readonly IQuestionOptionFactory _factory;
        private readonly IQuestionOptionValidator _validator;
        private readonly IOptionRepository _optionRepository;

        public QuestionOptionBuilderService(
            IQuestionOptionFactory factory,
            IQuestionOptionValidator validator,
            IOptionRepository optionRepository
        )
        {
            _factory = factory;
            _validator = validator;
            _optionRepository = optionRepository;
        }
        public async Task<Result<List<QuestionOptionBase>>> BuildQuestionOptions(List<OptionBaseDto> options, LearningQuestion question, QuestionType type)
        {
            var validateOptions = new List<QuestionOptionBase>();
            foreach(var questionOptionBase in options)
            {
                var option = await _optionRepository.GetOptionById(questionOptionBase.OptionId);
                if(option == null) return Result.Failure<List<QuestionOptionBase>>(OptionError.OptionNotFound);
                
                var questionOption = _factory.Create(type, question, option, 
                questionOptionBase.Order, questionOptionBase.IsCorrect, 
                questionOptionBase.SourceType, questionOptionBase.TargetType, 
                questionOptionBase.Position);    
                
                if(questionOption.IsFailure) return Result.Failure<List<QuestionOptionBase>>(questionOption.Error);
                validateOptions.Add(questionOption.Value);
            }

            var validate = _validator.Validate(type, validateOptions);
            if(validate.IsFailure) return Result.Failure<List<QuestionOptionBase>>(validate.Error);

            return Result.Success(validateOptions);
        }
    }
}