using Domain.Entities.Subscription;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Subscription
{
    public class SubscriptionConfiguration : IEntityTypeConfiguration<Domain.Entities.Subscription.Subscription>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.Subscription.Subscription> builder)
        {
            builder.HasKey(s => s.Id);

            builder.HasOne(s => s.SubscriptionType)
                .WithMany()
                .HasForeignKey("SubscriptionTypeId");
        }
    }
}