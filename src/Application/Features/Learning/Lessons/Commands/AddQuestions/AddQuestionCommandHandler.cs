using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions.Messaging;
using Application.Common.Interface;
using Application.Features.Learning.Lessons.Commands.AddQuestions.Services;
using Application.Features.Media.Commands.Upload;
using Application.Interface;
using Domain.Entities.Learning.Courses;
using Domain.Entities.Learning.Lessons;
using Domain.Entities.Learning.Questions;
using Domain.Entities.Learning.Questions.Configurations;
using Domain.Entities.Learning.Questions.Enums;
using Domain.Entities.Learning.Questions.Options;
using Domain.Entities.Learning.Questions.QuestionOptions;
using Domain.Entities.Learning.Questions.QuestionOptions.Factory;
using Domain.Entities.Learning.Questions.QuestionOptions.Validator;
using Domain.Entities.Media.Enums;
using Domain.Repositories;
using MediatR;
using SharedKernel;

namespace Application.Features.Learning.Lessons.Commands.AddQuestions
{
    public class AddQuestionCommandHandler : ICommandHandler<AddQuestionCommand>
    {
        private readonly IQuestionOptionFactory _factory;
        private readonly IQuestionOptionValidator _validator;
        private readonly ILessonRepository _lessonRepository;   
        private readonly IOptionRepository _optionRepository;
        private readonly IWordService _wordService;
        private readonly IQuestionBuilderService _questionBuilder;
        private readonly IQuestionOptionBuilderService _questionOptionBuilder;
        private readonly IApplicationDbContext _context;
        private readonly IMediator _mediator;
        public AddQuestionCommandHandler(
            IQuestionOptionFactory factory,
            IQuestionOptionValidator validator,
            ILessonRepository lessonRepository,
            IOptionRepository optionRepository,
            IQuestionBuilderService questionBuilder,
            IQuestionOptionBuilderService questionOptionBuilder,
            IMediator mediator,
            IWordService wordService,
            IApplicationDbContext context
        )
        {
            _factory = factory;
            _validator = validator;
            _lessonRepository = lessonRepository;
            _optionRepository = optionRepository;
            _questionBuilder = questionBuilder;
            _questionOptionBuilder = questionOptionBuilder;
            _mediator = mediator;
            _wordService = wordService;
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
            createQuestion.Value.AddOptions(createOptions.Value);

            lesson.AddQuestion(createQuestion.Value);
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}