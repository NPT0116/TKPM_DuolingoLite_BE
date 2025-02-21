
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities.Subscriptions;

namespace Infrastructure.Persistence.Configurations
{
    public class SubscriptionConfiguration : IEntityTypeConfiguration<Domain.Entities.Subscriptions.Subscription>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.Subscriptions.Subscription> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.SubscriptionType)
                .WithOne()
                .HasForeignKey<Domain.Entities.Subscriptions.Subscription>("SubscriptionTypeId");
        }
    }
}