using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions.Messaging;
using Application.Common.Interface;
using Application.Features.Media.Commands.Upload;
using Application.Interface;
using Domain.Entities.Learning.Courses;
using Domain.Entities.Learning.Lessons;
using Domain.Entities.Learning.Questions;
using Domain.Entities.Learning.Questions.Configurations;
using Domain.Entities.Learning.Questions.Enums;
using Domain.Entities.Learning.Questions.Options;
using Domain.Entities.Learning.Questions.QuestionOptions;
using Domain.Entities.Media.Enums;
using Domain.Repositories;
using MediatR;
using SharedKernel;

namespace Application.Features.Learning.Lessons.Commands.AddQuestions
{
    public class AddQuestionCommandHandler : ICommandHandler<AddQuestionCommand>
    {
        private readonly ILessonRepository _lessonRepository;   
        private readonly IOptionRepository _optionRepository;
        private readonly IMediaRepository _mediaRepository;
        private readonly ITextToSpeechService _textToSpeechService;
        private readonly IApplicationDbContext _context;
        private readonly IMediator _mediator;
        public AddQuestionCommandHandler(
            ILessonRepository lessonRepository,
            IOptionRepository optionRepository,
            IMediaRepository mediaRepository,
            ITextToSpeechService textToSpeechService,
            IMediator mediator,
            IApplicationDbContext context
        )
        {
            _lessonRepository = lessonRepository;
            _optionRepository = optionRepository;
            _mediaRepository = mediaRepository;
            _textToSpeechService = textToSpeechService;
            _mediator = mediator;
            _context = context;
        }
        public async Task<Result> Handle(AddQuestionCommand request, CancellationToken cancellationToken)
        {
            var (lessonId, question) = request;
            var (instruction, vietnameseText, englishText, image, audio, order, type, questionConfiguration, optionConfiguration, options) = question;
            
            var lesson = await _lessonRepository.GetLessonByIdAsync(lessonId);
            if(lesson == null) return Result.Failure(LessonError.LessonNotFound);

            if (new[]
            {
                (instruction, questionConfiguration.Instruction),
                (vietnameseText, questionConfiguration.VietnameseText),
                (englishText, questionConfiguration.EnglishText),
                (image, questionConfiguration.Image),
            }.Any(pair => (pair.Item1 == null && pair.Item2 == true) || (pair.Item1 != null && pair.Item2 == false)))
            {
                return Result.Failure(QuestionError.InvalidQuestionConfiguration);
            }

            Domain.Entities.Media.Media? questionAudio = null;
            if(questionConfiguration.Audio)
            {
                if(audio != null)
                {
                    questionAudio = Domain.Entities.Media.Media.Create(audio, MediaType.Audio,0, audio, audio).Value;
                }
                else
                {
                    if(englishText == null) return Result.Failure(QuestionError.InvalidQuestionConfiguration);
                    byte[] audioBytes = _textToSpeechService.GenerateAudioFileFromText(englishText);
                    var uploadRequest = new MediaUploadRequest(
                        string.Empty,
                        audioBytes,
                        englishText,
                        "audio/mp3"
                    );
                    var uploadCommand = new MediaUploadCommand(uploadRequest);
                    var uploadedFile = await _mediator.Send(uploadCommand);
                    if(uploadedFile.IsFailure) return Result.Failure(uploadedFile.Error);
                    questionAudio = uploadedFile.Value;
                }
            }


            Domain.Entities.Media.Media? questionImage = null;
            if(image != null)
            {
                questionImage = Domain.Entities.Media.Media.Create(image, MediaType.Image,0, image, image).Value;
            }

            var createQuestionConfiguration = Configuration.Create(
                questionConfiguration.Audio,
                questionConfiguration.EnglishText,
                questionConfiguration.VietnameseText,
                questionConfiguration.Instruction,
                questionConfiguration.Image
            );

            if(createQuestionConfiguration.IsFailure) return Result.Failure(createQuestionConfiguration.Error);

            var createOptionConfiguration = Configuration.Create(
                optionConfiguration.Audio,
                optionConfiguration.EnglishText,
                optionConfiguration.VietnameseText,
                optionConfiguration.Instruction,
                optionConfiguration.Image
            );

            if(createOptionConfiguration.IsFailure) return Result.Failure(createOptionConfiguration.Error);

            var createQuestion = Domain.Entities.Learning.Questions.Question.Create(
                instruction,
                vietnameseText,
                questionAudio,
                englishText,
                questionImage,
                type,
                createQuestionConfiguration.Value,
                createOptionConfiguration.Value,
                lesson.Questions.Count() + 1
            );

            if(createQuestion.IsFailure) return Result.Failure(createQuestion.Error);

            foreach(var questionOptionBase in options)
            {
                var option = await _optionRepository.GetOptionById(questionOptionBase.OptionId);
                if(option == null) return Result.Failure(OptionError.OptionNotFound);
                
                QuestionOptionBase questionOption = null;
                if(type == QuestionType.MultipleChoice)
                {
                    var multipleChoiceOption = (MultipleChoiceOptionBaseDto) questionOptionBase;
                    var questionOptionCreate = MultipleChoiceQuestionOption.Create(
                        createQuestion.Value,
                        option,
                        multipleChoiceOption.IsCorrect,
                        multipleChoiceOption.Order
                    );

                    if(questionOptionCreate.IsFailure) return Result.Failure(questionOptionCreate.Error);
                    questionOption = questionOptionCreate.Value;
                }
                else
                {
                    // createQuestion.Value.AddOption(option);
                }

                createQuestion.Value.AddOption(questionOption);
            }

            lesson.AddQuestion(createQuestion.Value);
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}