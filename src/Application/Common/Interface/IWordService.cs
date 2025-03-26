using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Learning.Words.Queries.GetWordDefinition;

namespace Application.Common.Interface
{
    public enum Language
    {
        en,
        vi
    }
    public interface IWordService
    {
        public Task<List<string>> SplitWordsFromString(string prompt, Language language = Language.en);
        Task<List<WordDefinitionDto>> GetWordDefinition(string word);
    }
}