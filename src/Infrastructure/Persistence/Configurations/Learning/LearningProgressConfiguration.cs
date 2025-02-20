using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Learning.LearningProgress;
using Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Learning
{
    public class LearningProgressConfiguration : IEntityTypeConfiguration<LearningProgress>
    {
        public void Configure(EntityTypeBuilder<LearningProgress> builder)
        {
            builder.HasKey(lp => lp.Id);

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey("userId");
                
            builder.HasOne(lp => lp.Course)
                .WithMany()
                .HasForeignKey("CourseId");
        }
    }
}