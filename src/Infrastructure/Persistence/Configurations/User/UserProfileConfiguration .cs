using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.User;
using Domain.Entities.User.ValueObjects;
using Domain.Entities.Users;
using Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.User
{
    public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.HasKey(up => up.Id);

        builder.HasOne<ApplicationUser>()
            .WithOne()
            .HasForeignKey<UserProfile>(up => up.UserId);

        builder.Property(up => up.Email)
        .HasConversion(
            email => email != null ? email.Value : null,
            value => value != null ? Email.Create(value).Value : null
        )
        .IsRequired(false);  // Ensures that the Email property is nullable

    }
}

}