using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interface;
using Application.Features.Media.Commands.Upload;
using Domain.Entities.Learning.Questions;
using Domain.Entities.Learning.Questions.Configurations;
using Domain.Entities.Learning.Questions.Enums;
using Domain.Entities.Learning.Questions.QuestionOptions.Factory;
using Domain.Entities.Media.Enums;
using Domain.Repositories;
using MediatR;
using SharedKernel;
using LearningQuestion = Domain.Entities.Learning.Questions.Question;



namespace Application.Features.Learning.Lessons.Commands.AddQuestions.Services
{
    public class QuestionBuilderService : IQuestionBuilderService
    {
        private readonly IMediaRepository _mediaRepository;
        private readonly IQuestionFactory _factory;
        private readonly ITextToSpeechService _textToSpeechService;
        private readonly IMediator _mediator;
        public QuestionBuilderService(
            IMediaRepository mediaRepository,
            IQuestionFactory factory,
            ITextToSpeechService textToSpeechService,
            IMediator mediator)
        {
            _mediaRepository = mediaRepository;
            _factory = factory;
            _textToSpeechService = textToSpeechService;
            _mediator = mediator;
        }
        public async Task<Result<LearningQuestion>> BuildQuestion(string? instruction, string? vietnameseText, 
            string? englishText, string? image, string? audio, 
            QuestionType type, ConfigurationDto questionConfiguration, 
            ConfigurationDto optionConfiguration, int order)
        {
            

            Domain.Entities.Media.Media? questionAudio = null;
            if(questionConfiguration.Audio)
            {
                if(audio != null)
                {
                    var createAudio = await _mediaRepository.UploadFileAsync(audio, audio, MediaType.Audio, 10, DateTime.UtcNow, DateTime.UtcNow, audio, CancellationToken.None);
                    if(createAudio.IsFailure) return Result.Failure<LearningQuestion>(createAudio.Error);
                    questionAudio = createAudio.Value;
                }
                else
                {
                    if(englishText == null) return Result.Failure<LearningQuestion>(QuestionError.InvalidQuestionConfiguration);
                    byte[] audioBytes = _textToSpeechService.GenerateAudioFileFromText(englishText);
                    var uploadRequest = new MediaUploadRequest(
                        string.Empty,
                        audioBytes,
                        englishText,
                        "audio/mp3"
                    );
                    var uploadCommand = new MediaUploadCommand(uploadRequest);
                    var uploadedFile = await _mediator.Send(uploadCommand);
                    if(uploadedFile.IsFailure) return Result.Failure<LearningQuestion>(uploadedFile.Error);
                    questionAudio = uploadedFile.Value;
                }
            }


            Domain.Entities.Media.Media? questionImage = null;
            if(image != null)
            {
                var createImage = await _mediaRepository.UploadFileAsync(image, image, MediaType.Image, 10, DateTime.UtcNow, DateTime.UtcNow, image, CancellationToken.None);
                if(createImage.IsFailure) return Result.Failure<LearningQuestion>(createImage.Error);
                questionImage = createImage.Value;
            }

            var createQuestionConfiguration = Configuration.Create(
                questionConfiguration.Audio,
                questionConfiguration.EnglishText,
                questionConfiguration.VietnameseText,
                questionConfiguration.Instruction,
                questionConfiguration.Image
            );
            if(createQuestionConfiguration.IsFailure) return Result.Failure<LearningQuestion>(createQuestionConfiguration.Error);

            var createOptionConfiguration = Configuration.Create(
                optionConfiguration.Audio,
                optionConfiguration.EnglishText,
                optionConfiguration.VietnameseText,
                optionConfiguration.Instruction,
                optionConfiguration.Image
            );
            if(createOptionConfiguration.IsFailure) return Result.Failure<LearningQuestion>(createOptionConfiguration.Error);

            var createQuestion = _factory.Create(
                instruction,
                vietnameseText,
                questionAudio,
                englishText,
                questionImage,
                type,
                createQuestionConfiguration.Value,
                createOptionConfiguration.Value,
                order
            );

            if(createQuestion.IsFailure) return Result.Failure<LearningQuestion>(createQuestion.Error);
            return Result.Success(createQuestion.Value);
        }
    }
}