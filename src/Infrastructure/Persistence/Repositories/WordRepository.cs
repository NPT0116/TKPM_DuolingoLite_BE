using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Learning.Words;
using Domain.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class WordRepository : IWordRepository
    {
        private readonly ApplicationDbContext _context;
        public WordRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Word?> AddWord(Word word)
        {
            var addedWord = await _context.Words.AddAsync(word);
            return addedWord.Entity;
        }

        public async Task<Word?> FindWord(string word)
        {
            return await _context.Words.FirstOrDefaultAsync(w => w.Content.Trim().ToLower() == word.Trim().ToLower());
        }
    }
}