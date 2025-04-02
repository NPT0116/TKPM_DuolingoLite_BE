
using Application.Common.Interface;
using MediatR;

namespace Application.Features.Learning.Words.Queries.GetWordDefinition
{
    public class GetWordDefinitionQueryHandler : IRequestHandler<GetWordDefinitionQuery, List<WordDefinitionDto>>
    {
        private readonly IWordService _wordService;
        public GetWordDefinitionQueryHandler(IWordService wordService)
        {
            _wordService = wordService;
        }
        public async Task<List<WordDefinitionDto>> Handle(GetWordDefinitionQuery request, CancellationToken cancellationToken)
        {
            var definitions = await _wordService.GetWordDefinition(request.Word);
            return definitions;
        }
    }
}