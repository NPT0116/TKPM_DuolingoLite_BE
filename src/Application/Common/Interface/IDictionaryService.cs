using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Learning.Words.Queries.GetWordDefinition;

namespace Application.Common.Interface
{
    public interface IDictionaryService
    {
        Task<List<WordDefinitionDto>> GetWordDefinition(string word);
    }
}