using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Learning.Courses;
using Domain.Entities.Learning.Lessons;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class LessonConfiguration : IEntityTypeConfiguration<Lesson>
    {
        public void Configure(EntityTypeBuilder<Lesson> builder)
        {
            builder.HasKey(l => l.Id);
            builder.Property("CourseId")
                .IsRequired();

            builder.HasOne<Course>()
                .WithMany(c => c.Lessons)
                .HasForeignKey("CourseId");

            builder.Property(l => l.Title)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(l => l.XpEarned)
                .IsRequired();

            builder.Property(l => l.Order)
                .IsRequired();

            builder.HasMany(l => l.Questions)
                .WithOne()
                .HasForeignKey("LessonId");
        }
    }
}