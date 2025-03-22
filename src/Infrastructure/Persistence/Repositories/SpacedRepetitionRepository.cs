using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Learning.SpacedRepetition;
using Domain.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class SpacedRepetitionRepository : ISpacedRepetitionRepository
    {
        private readonly ApplicationDbContext _context;

        public SpacedRepetitionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SpacedRepetitionRecord?> GetByIdAsync(Guid id)
        {
            return await _context.SpacedRepetitionRecords
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<SpacedRepetitionRecord?> GetByUserAndQuestionAsync(Guid userId, Guid questionId)
        {
            return await _context.SpacedRepetitionRecords
                .FirstOrDefaultAsync(r => r.UserId == userId && r.QuestionId == questionId);
        }

        public async Task<List<SpacedRepetitionRecord>> GetDueReviewsAsync(Guid userId, int limit, DateTime? cursor)
        {
            var now = DateTime.UtcNow;
            
            var query = _context.SpacedRepetitionRecords
                .Where(r => r.UserId == userId && r.NextReview <= now);

            // Apply cursor-based pagination if cursor is provided
            if (cursor.HasValue)
            {
                query = query.Where(r => r.NextReview > cursor.Value);
            }

            // Order by NextReview ascending to prioritize oldest due reviews
            query = query.OrderBy(r => r.NextReview);

            // Limit the results
            return await query.Take(limit).ToListAsync();
        }

        public async Task<SpacedRepetitionRecord?> AddAsync(SpacedRepetitionRecord record)
        {
            await _context.SpacedRepetitionRecords.AddAsync(record);
            await _context.SaveChangesAsync();
            return record;
        }

        public async Task UpdateAsync(SpacedRepetitionRecord record)
        {
            _context.SpacedRepetitionRecords.Update(record);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(Guid userId, Guid questionId)
        {
            return await _context.SpacedRepetitionRecords
                .AnyAsync(r => r.UserId == userId && r.QuestionId == questionId);
        }
    }
} 