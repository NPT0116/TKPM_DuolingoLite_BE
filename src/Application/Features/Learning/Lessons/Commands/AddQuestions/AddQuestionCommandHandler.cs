using Application.Abstractions.Messaging;
using Application.Common.Interface;
using Application.Features.Learning.Lessons.Commands.AddQuestions.Services;
using Application.Interface;
using Domain.Entities.Learning.Lessons;
using Domain.Entities.Media.Constants;
using Domain.Repositories;
using MediatR;
using SharedKernel;

namespace Application.Features.Learning.Lessons.Commands.AddQuestions
{
    public class AddQuestionCommandHandler : ICommandHandler<AddQuestionCommand>
    {
        private readonly ILessonRepository _lessonRepository;
        private readonly IQuestionBuilderService _questionBuilder;
        private readonly IQuestionOptionBuilderService _questionOptionBuilder;
        private readonly IWordGeneratorService _wordGenerator;
        private readonly IMediaStorageService _storage;
        private readonly IApplicationDbContext _context;
        public AddQuestionCommandHandler(
            ILessonRepository lessonRepository,
            IQuestionBuilderService questionBuilder,
            IQuestionOptionBuilderService questionOptionBuilder,
            IWordGeneratorService wordGenerator,
            IMediaStorageService storage,
            IApplicationDbContext context
        )
        {
            _lessonRepository = lessonRepository;
            _questionBuilder = questionBuilder;
            _questionOptionBuilder = questionOptionBuilder;
            _wordGenerator = wordGenerator;
            _storage = storage;
            _context = context;
        }
        public async Task<Result> Handle(AddQuestionCommand request, CancellationToken cancellationToken)
        {
            using var transaction = await _context.BeginTransactionAsync();
            var startTime = DateTime.UtcNow;
            var createQuestionResult = await CreateQuestion(request, cancellationToken);
            if(createQuestionResult.IsFailure)
            {
                await _context.RollbackTransactionAsync(transaction);
                var foldersToClean = new List<string>() { MediaConstants.Question, MediaConstants.QuestionOption, ""};
                await _storage.ScanAsync(foldersToClean, startTime);

                var audio = request.question.Audio;
                if(!string.IsNullOrEmpty(audio)) await _storage.DeleteFileFromUrl(audio);

                var image = request.question.Image;
                if(!string.IsNullOrEmpty(image)) await _storage.DeleteFileFromUrl(image);

                return createQuestionResult;
            }
            await _context.CommitTransactionAsync(transaction);
            return createQuestionResult;
        }

        private async Task<Result> CreateQuestion(AddQuestionCommand request, CancellationToken cancellationToken)
        {
            var (lessonId, question) = request;
            var (instruction, vietnameseText, englishText, image, audio, sentence, order, type, questionConfiguration, optionConfiguration, options) = question;
            
            var lesson = await _lessonRepository.GetLessonByIdAsync(lessonId);
            if(lesson == null) return Result.Failure(LessonError.LessonNotFound);


            var createQuestion = await _questionBuilder.BuildQuestion(
                instruction, vietnameseText, englishText, 
                image, audio, type, questionConfiguration, optionConfiguration, lesson.Questions.Count() + 1
            );
            if(createQuestion.IsFailure) return Result.Failure(createQuestion.Error);

            var createOptions = await _questionOptionBuilder.BuildQuestionOptions(options, createQuestion.Value, type, sentence);
            if(createOptions.IsFailure) return Result.Failure(createOptions.Error);
            var questionOptions = createOptions.Value;
            
            if(options.Any()) createQuestion.Value.AddOptions(questionOptions);

            lesson.AddQuestion(createQuestion.Value);

            if(englishText != null) await _wordGenerator.GenerateWords(englishText);
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}