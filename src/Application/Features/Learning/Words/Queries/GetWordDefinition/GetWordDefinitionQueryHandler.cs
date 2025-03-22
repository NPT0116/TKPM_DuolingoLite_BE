
using IDictionaryService = Application.Common.Interface.IDictionaryService;
using MediatR;

namespace Application.Features.Learning.Words.Queries.GetWordDefinition
{
    public class GetWordDefinitionQueryHandler : IRequestHandler<GetWordDefinitionQuery, List<WordDefinitionDto>>
    {
        private readonly IDictionaryService _dictionaryService;
        public GetWordDefinitionQueryHandler(IDictionaryService dictionaryService)
        {
            _dictionaryService = dictionaryService;
        }
        public async Task<List<WordDefinitionDto>> Handle(GetWordDefinitionQuery request, CancellationToken cancellationToken)
        {
            var definitions = await _dictionaryService.GetWordDefinition(request.Word);
            return definitions;
        }
    }
}