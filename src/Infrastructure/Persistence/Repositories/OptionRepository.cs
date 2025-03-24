using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Interface;
using Domain.Entities.Learning.Questions.Options;
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

        public async Task<Option?> GetOptionById(Guid optionId)
        {
            return await _context.Options
                .FirstOrDefaultAsync(o => o.Id == optionId);
        }
    }
}