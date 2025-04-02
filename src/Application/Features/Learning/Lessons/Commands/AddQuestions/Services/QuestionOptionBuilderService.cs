using Application.Common.Interface;
using Application.Features.Media.Commands.Upload;
using Domain.Entities.Learning.Questions.Enums;
using Domain.Entities.Learning.Questions.Options;
using Domain.Entities.Learning.Questions.QuestionOptions;
using Domain.Entities.Learning.Questions.QuestionOptions.Factory;
using Domain.Entities.Learning.Questions.QuestionOptions.Validator;
using Domain.Entities.Learning.Words.Enums;
using Domain.Entities.Media.Constants;
using Domain.Repositories;
using MediatR;
using SharedKernel;
using LearningQuestion = Domain.Entities.Learning.Questions.Question;
namespace Application.Features.Learning.Lessons.Commands.AddQuestions.Services
{
    public class QuestionOptionBuilderService : IQuestionOptionBuilderService
    {
        private readonly IQuestionOptionFactory _factory;
        private readonly IQuestionOptionValidator _validator;
        private readonly IOptionRepository _optionRepository;
        private readonly IWordService _wordService;
        private readonly ITextToSpeechService _textToSpeechService;

        public QuestionOptionBuilderService(
            IQuestionOptionFactory factory,
            IQuestionOptionValidator validator,
            IOptionRepository optionRepository,
            IWordService wordService,
            ITextToSpeechService textToSpeechService
        )
        {
            _factory = factory;
            _validator = validator;
            _optionRepository = optionRepository;
            _wordService = wordService;
            _textToSpeechService = textToSpeechService;
        }
        public async Task<Result<List<QuestionOptionBase>>> BuildQuestionOptions(
            List<OptionBaseDto> options, LearningQuestion question, 
            QuestionType type, string? sentence)
        {
            return type switch
            {
                QuestionType.Matching => await BuildMatchingQuestionOptions(options, question),
                QuestionType.MultipleChoice => await BuildMultiplecChoiceQuestionOptions(options, question),
                QuestionType.Pronunciation => BuildPronunciationQuestionOptions(),
                QuestionType.BuildSentence => await BuildBuildSentenceQuestionOptions(options, question, sentence),
                _ => Result.Failure<List<QuestionOptionBase>>(QuestionOptionError.QuestionTypeNotSupported)
            };
        }

        private async Task<Result<List<QuestionOptionBase>>> BuildQuestionOptionsFromList(
            List<OptionBaseDto> options,
            LearningQuestion question,
            QuestionType type)
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

        private async Task<Result<List<QuestionOptionBase>>> BuildMatchingQuestionOptions(
            List<OptionBaseDto> options,
            LearningQuestion question)
        {
            return await BuildQuestionOptionsFromList(options, question, QuestionType.Matching);
        }

        private async Task<Result<List<QuestionOptionBase>>> BuildMultiplecChoiceQuestionOptions(
            List<OptionBaseDto> options,
            LearningQuestion question)
        {
            return await BuildQuestionOptionsFromList(options, question, QuestionType.MultipleChoice);
        }

        private Result<List<QuestionOptionBase>> BuildPronunciationQuestionOptions()
        {
            return Result.Success(new List<QuestionOptionBase>());    
        }

        private async Task<Result<List<QuestionOptionBase>>> BuildBuildSentenceQuestionOptions(
            List<OptionBaseDto> options,
            LearningQuestion question,
            string? sentence
        )
        {
            if(string.IsNullOrEmpty(sentence)) return Result.Failure<List<QuestionOptionBase>>(QuestionOptionError.NoOptions);

            var isSourceTypeEnglish = !string.IsNullOrEmpty(question.EnglishText);
            var targetLanguage = isSourceTypeEnglish ? Language.vi : Language.en;
            Console.WriteLine(targetLanguage);
            var words = await _wordService.SplitWordsFromString(sentence, targetLanguage);

            var optionList = new List<QuestionOptionBase>();
            var position = 1;
            foreach(var word in words)
            {
                var option = await _optionRepository.FindOptionThatExactlyMatches(word, targetLanguage);
                if(option == null)
                {
                    Domain.Entities.Media.Media? audio = null;
                    if(targetLanguage == Language.en)
                    {
                        var createAudio = await _textToSpeechService.GenerateMediaFromText(word, MediaConstants.Option);
                        if(createAudio.IsSuccess) audio = createAudio.Value;
                    }
                    var newOption = Option.Create(
                        targetLanguage == Language.vi ? word : null, 
                        null, 
                        audio, 
                        targetLanguage == Language.en ? word : null);

                    if(newOption.IsFailure) continue;
                    option = newOption.Value;
                    await _optionRepository.CreateOption(option);
                }

                var questionOption = _factory.Create(
                    QuestionType.BuildSentence, question, 
                    option, position, null, null, null, position++);
                if(questionOption.IsFailure) return Result.Failure<List<QuestionOptionBase>>(questionOption.Error);
                optionList.Add(questionOption.Value);
            }

            foreach(var option in options)
            {
                var existingOption = await _optionRepository.GetOptionById(option.OptionId);
                if(existingOption == null) continue;
                var questionOption = _factory.Create(QuestionType.BuildSentence, question, 
                    existingOption, position, null, null, null, position++);
                optionList.Add(questionOption.Value);
            }

            return optionList;
        }
    }
}