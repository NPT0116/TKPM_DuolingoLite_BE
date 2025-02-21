using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Learning.Questions.QuestionOptions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Configurations.Learning
{
    public class QuestionWordConfiguration : IEntityTypeConfiguration<QuestionWord>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<QuestionWord> builder)
        {
            builder.HasKey(w => w.Id);

            builder.HasOne(w => w.Word)
                .WithMany()
                .HasForeignKey("WordId");

            builder.HasOne(w => w.Question)
                .WithMany(q => q.Words)
                .HasForeignKey("QuestionId");
        }
    }
}