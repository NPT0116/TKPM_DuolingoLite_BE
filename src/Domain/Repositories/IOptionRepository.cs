using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Learning.Questions.Options;
using Domain.Entities.Learning.Words.Enums;

namespace Domain.Repositories
{
    public interface IOptionRepository
    {
        Task<Option?> FindOption(string? englishText, string? vietnameseText);
        Task<Option> CreateOption(Option option);
        Task<Option?> GetOptionById(Guid optionId);
        Task<List<Option>> GetOptionsByEnglishText(string englishText);
        Task<List<Option>> GetOptionsByVietnameseText(string vietnameseText);
        Task<Option?> FindOptionThatExactlyMatches(string text, Language language);
        void DeleteOption(Option option);
    }
}