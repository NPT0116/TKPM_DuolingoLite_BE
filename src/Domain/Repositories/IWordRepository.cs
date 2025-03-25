using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Learning.Words;

namespace Domain.Repositories
{
    public interface IWordRepository
    {
        Task<Word?> AddWord(Word word); 
        Task<Word?> FindWord(string word);
    }
}