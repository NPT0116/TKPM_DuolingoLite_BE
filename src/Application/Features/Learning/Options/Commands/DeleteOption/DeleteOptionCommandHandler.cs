using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions.Messaging;
using Application.Interface;
using Domain.Entities.Learning.Questions.Options;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.Learning.Options.Commands.DeleteOption
{
    public class DeleteOptionCommandHandler : ICommandHandler<DeleteOptionCommand>
    {
        private readonly IOptionRepository _optionRepository;
        private readonly IQuestionOptionRepository _questionOptionRepostiory;
        private readonly IApplicationDbContext _context;
        public DeleteOptionCommandHandler(
            IOptionRepository optionRepository,
            IQuestionOptionRepository questionOptionRepository,
            IApplicationDbContext context)
        {
            _optionRepository = optionRepository;
            _questionOptionRepostiory = questionOptionRepository;
            _context = context;
        }
        public async Task<Result> Handle(DeleteOptionCommand request, CancellationToken cancellationToken)
        {
            var option = await _optionRepository.GetOptionById(request.optionId);
            if(option == null) return Result.Failure(OptionError.OptionNotFound);

            var questionsCount = await _questionOptionRepostiory.GetQuestionsCountByOptionAsync(request.optionId);
            
            if(questionsCount > 0)
            {
                return Result.Failure(OptionError.OptionHasBeenUsed);
            }

            _optionRepository.DeleteOption(option);
            await _context.SaveChangesAsync();
            return Result.Success();
        }
    }
}