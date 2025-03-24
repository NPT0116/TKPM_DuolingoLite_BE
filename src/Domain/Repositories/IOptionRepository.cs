using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Learning.Questions.Options;

namespace Domain.Repositories
{
    public interface IOptionRepository
    {
        Task<Option?> FindOption(string? englishText, string? vietnameseText);
        Task<Option> CreateOption(Option option);
        Task<Option?> GetOptionById(Guid optionId);
    }
}