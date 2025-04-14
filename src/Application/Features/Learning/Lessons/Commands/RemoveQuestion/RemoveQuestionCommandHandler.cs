using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions.Messaging;
using Application.Interface;
using Domain.Entities.Learning.Lessons;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.Learning.Lessons.Commands.RemoveQuestion
{
    public class RemoveQuestionCommandHandler : ICommandHandler<RemoveQuestionCommand>
    {
        private readonly ILessonRepository _lessonRepository;
        private readonly ISpacedRepetitionRepository _spacedRepetitionRepository;
        private readonly IApplicationDbContext _context;
        public RemoveQuestionCommandHandler(
            ILessonRepository lessonRepository,
            ISpacedRepetitionRepository spacedRepetitionRepository,
            IApplicationDbContext context)
        {
            _context = context;
            _lessonRepository = lessonRepository;
            _spacedRepetitionRepository = spacedRepetitionRepository;
        }
        public async Task<Result> Handle(RemoveQuestionCommand request, CancellationToken cancellationToken)
        {
            var lesson = await _lessonRepository.GetLessonByIdAsync(request.lessonId);
            if(lesson == null)
            {
                return Result.Failure(LessonError.LessonNotFound);
            }

            var removeQuestion = lesson.RemoveQuestion(request.questionOrder);
            if(removeQuestion.IsFailure)
            {
                return Result.Failure(removeQuestion.Error);
            }

            var question = lesson.GetQuestionByOrder(request.questionOrder);
            if(question == null)
            {
                return Result.Failure(LessonError.QuestionOrderNotFound(request.questionOrder, request.lessonId));
            }

            var spacedRepetitionRecordCount = await _spacedRepetitionRepository.GetSpacedRepetitionRecordCountForQuestionAsync(question.Value.Id);
            if(spacedRepetitionRecordCount > 0)
            {
                return Result.Failure(LessonError.QuestionHasBeenUsedInSpacedRepetition(request.questionOrder,request.lessonId));
            }

            await _context.SaveChangesAsync();
            return Result.Success();
        }
    }
}