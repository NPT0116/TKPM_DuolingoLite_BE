using System;
using Domain.Entities.Learning.SpacedRepetition;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class SpacedRepetitionRecordConfiguration : IEntityTypeConfiguration<SpacedRepetitionRecord>
    {
        public void Configure(EntityTypeBuilder<SpacedRepetitionRecord> builder)
        {
            builder.HasKey(x => x.Id);
            
            // Create a composite index for faster lookups by user and question
            builder.HasIndex(x => new { x.UserId, x.QuestionId })
                .IsUnique();
            
            // Create an index for finding due reviews by user and next review date
            builder.HasIndex(x => new { x.UserId, x.NextReview });
        }
    }
} 