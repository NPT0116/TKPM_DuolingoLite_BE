using Application.Abstractions.Messaging;
using Application.Features.Learning.Lessons.Commands.AddQuestions.Services;
using Application.Interface;
using Domain.Entities.Learning.Lessons;
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
        private readonly IApplicationDbContext _context;
        public AddQuestionCommandHandler(
            ILessonRepository lessonRepository,
            IQuestionBuilderService questionBuilder,
            IQuestionOptionBuilderService questionOptionBuilder,
            IWordGeneratorService wordGenerator,
            IApplicationDbContext context
        )
        {
            _lessonRepository = lessonRepository;
            _questionBuilder = questionBuilder;
            _questionOptionBuilder = questionOptionBuilder;
            _wordGenerator = wordGenerator;
            _context = context;
        }
        public async Task<Result> Handle(AddQuestionCommand request, CancellationToken cancellationToken)
        {
            var (lessonId, question) = request;
            var (instruction, vietnameseText, englishText, image, audio, order, type, questionConfiguration, optionConfiguration, options) = question;
            
            var lesson = await _lessonRepository.GetLessonByIdAsync(lessonId);
            if(lesson == null) return Result.Failure(LessonError.LessonNotFound);


            var createQuestion = await _questionBuilder.BuildQuestion(
                instruction, vietnameseText, englishText, 
                image, audio, type, questionConfiguration, optionConfiguration, lesson.Questions.Count() + 1
            );
            if(createQuestion.IsFailure) return Result.Failure(createQuestion.Error);

            var createOptions = await _questionOptionBuilder.BuildQuestionOptions(options, createQuestion.Value, type);
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