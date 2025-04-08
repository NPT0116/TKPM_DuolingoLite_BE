using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities.Learning.SpacedRepetition;

namespace Domain.Repositories
{
    public interface ISpacedRepetitionRepository
    {
        Task<SpacedRepetitionRecord?> GetByIdAsync(Guid id);
        Task<SpacedRepetitionRecord?> GetByUserAndQuestionAsync(Guid userId, Guid questionId);
        Task<List<SpacedRepetitionRecord>> GetDueReviewsAsync(Guid userId, int limit, DateTime? cursor);
        Task<SpacedRepetitionRecord?> AddAsync(SpacedRepetitionRecord record);
        Task UpdateAsync(SpacedRepetitionRecord record);
        Task<bool> ExistsAsync(Guid userId, Guid questionId);
        Task<int> GetSpacedRepetitionRecordCountForQuestionAsync(Guid questionId);

    }
} 