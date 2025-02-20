
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class SubscriptionConfiguration : IEntityTypeConfiguration<Domain.Entities.Subscription.Subscription>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.Subscription.Subscription> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.SubscriptionType)
                .WithOne()
                .HasForeignKey<Domain.Entities.Subscription.Subscription>("SubscriptionTypeId");
        }
    }
}