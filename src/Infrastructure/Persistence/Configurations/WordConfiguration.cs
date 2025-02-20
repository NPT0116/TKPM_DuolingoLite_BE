using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Learning.Words;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class WordConfiguration : IEntityTypeConfiguration<Word>
    {
        public void Configure(EntityTypeBuilder<Word> builder)
        {
            builder.HasKey(w => w.Id);

            builder.Property(w => w.Content).IsRequired().HasMaxLength(100);

            builder.HasOne(w => w.Image)
                .WithMany()
                .HasForeignKey("ImageId")
                .IsRequired(false);

            builder.HasOne(w => w.Audio)
                .WithMany()
                .HasForeignKey("AudioId")
                .IsRequired(false);
        }
    }
}