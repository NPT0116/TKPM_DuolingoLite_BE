using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Interface;
using Domain.Entities.Learning.Questions.Options;
using Domain.Entities.Learning.Words.Enums;
using Domain.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class OptionRepository : IOptionRepository
    {
        private readonly ApplicationDbContext _context;
        public OptionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Option> CreateOption(Option option)
        {
            await _context.Options.AddAsync(option);
            return option;
        }

        public async Task<Option?> FindOption(string? englishText, string? vietnameseText)
        {
            return await _context.Options
                .FirstOrDefaultAsync(o => o.EnglishText == englishText && o.VietnameseText == vietnameseText);
        }

        public async Task<Option?> FindOptionThatExactlyMatches(string text, Language language)
        {
            var processText = text.Trim().ToLower();
            return await _context.Options.FirstOrDefaultAsync(
                o => language == Language.en
                ? o.EnglishText == processText 
                : o.VietnameseText == processText);
        }

        public async Task<Option?> GetOptionById(Guid optionId)
        {
            return await _context.Options
                .FirstOrDefaultAsync(o => o.Id == optionId);
        }

        public async Task<List<Option>> GetOptionsByEnglishText(string englishText)
        {
            var options = await _context.Options
                .Where(o => o.EnglishText != null && o.EnglishText.Trim().ToLower().Contains(englishText.Trim().ToLower()))
                .ToListAsync();
            return options;
        }

        public async Task<List<Option>> GetOptionsByVietnameseText(string vietnameseText)
        {
            var options = await _context.Options
                .Where(o => o.VietnameseText != null && o.VietnameseText.Trim().ToLower().Contains(vietnameseText.Trim().ToLower()))
                .ToListAsync();
            return options;
        }
    }
}