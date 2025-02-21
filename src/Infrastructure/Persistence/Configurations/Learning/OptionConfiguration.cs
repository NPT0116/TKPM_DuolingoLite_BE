using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Learning.Questions.Options;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Configurations.Learning
{
    public class OptionConfiguration : IEntityTypeConfiguration<Option>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Option> builder)
        {
            builder.HasKey(o => o.Id);

            builder.HasOne(o => o.Image)
                .WithMany()
                .HasForeignKey("ImageId")
                .IsRequired(false);

            builder.HasOne(o => o.Audio)
                .WithMany()
                .HasForeignKey("AudioId")
                .IsRequired(false);
        }
    }
}