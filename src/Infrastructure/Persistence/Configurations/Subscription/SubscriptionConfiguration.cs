using Domain.Entities.Subscriptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Subscription
{
    public class SubscriptionConfiguration : IEntityTypeConfiguration<Domain.Entities.Subscriptions.Subscription>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.Subscriptions.Subscription> builder)
        {
            builder.HasKey(s => s.Id);

            builder.HasOne(s => s.SubscriptionType)
                .WithMany()
                .HasForeignKey("SubscriptionTypeId");
        }
    }
}