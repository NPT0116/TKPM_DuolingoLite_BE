using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Course;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            builder.HasKey(c => c.Id);
            
            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.Level)
                .IsRequired();
                
            builder.HasOne(c => c.NextCourse)
                .WithOne()
                .HasForeignKey<Course>("NextCourseId")
                .IsRequired(false);

            builder.HasMany(c => c.Lessons)
                .WithOne()
                .HasForeignKey("CourseId");
            
        }
    }
}