using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions.Messaging;
using Domain.Entities.Learning.Questions.Options;
using Domain.Repositories;
using MediatR;
using SharedKernel;

namespace Application.Features.Learning.Options.Queries.FindOptionByEnglish
{
    public class FindOptionByEnglishQueryHandler : IQueryHandler<FindOptionByEnglishQuery, List<Option>>
    {
        private readonly IOptionRepository _optionRepository;
        public FindOptionByEnglishQueryHandler(IOptionRepository optionRepository)
        {
            _optionRepository = optionRepository;
        }

        async Task<Result<List<Option>>> IRequestHandler<FindOptionByEnglishQuery, Result<List<Option>>>.Handle(FindOptionByEnglishQuery request, CancellationToken cancellationToken)
        {
            return await _optionRepository.GetOptionsByEnglishText(request.EnglishText);
        }
    }
}