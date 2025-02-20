using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Question;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Configurations.Learning
{
    public class QuestionConfiguration : IEntityTypeConfiguration<Question>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Question> builder)
        {
            builder.HasKey(q => q.Id);

            builder.HasMany(q => q.Options)
                .WithOne(o => o.Question)
                .HasForeignKey("QuestionId");

            builder.HasOne(q => q.QuestionConfiguration)
                .WithOne()
                .HasForeignKey<Question>("QuestionConfigurationId");

            builder.HasOne(q => q.OptionConfiguration)
                .WithOne()
                .HasForeignKey<Question>("OptionConfigurationId");

            builder.HasMany(q => q.Words)
                .WithOne(w => w.Question)
                .HasForeignKey("QuestionId")
                .IsRequired(false);
        }
    }
}