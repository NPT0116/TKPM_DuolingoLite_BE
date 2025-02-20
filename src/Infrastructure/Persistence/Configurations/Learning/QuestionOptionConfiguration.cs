using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Question.QuestionOption;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Configurations.Learning
{
    public class QuestionOptionConfiguration : IEntityTypeConfiguration<QuestionOptionBase>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<QuestionOptionBase> builder)
        {
            builder.ToTable("QuestionOptions");
            
            builder.HasDiscriminator<string>("OptionType")
                .HasValue<MultipleChoiceQuestionOption>("MultipleChoice")
                .HasValue<MatchingQuestionOption>("Matching")
                .HasValue<BuildSentenceQuestionOption>("BuildSentence")
                .HasValue<PronunciationQuestionOption>("Pronunciation");

            builder.HasOne(o => o.Question)
                    .WithMany(q => q.Options)
                    .HasForeignKey("QuestionId");

            builder.HasOne(o => o.Option)
                    .WithMany()
                    .HasForeignKey("OptionId");
        }
    }
}