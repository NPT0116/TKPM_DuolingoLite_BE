using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharedKernel;

namespace Domain.Entities.Subscriptions
{
    public class Subscription : Entity
    {
        public SubscriptionType SubscriptionType { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime ExpiredDate { get; private set; }
        
        private Subscription() {}

        private Subscription(
            SubscriptionType subscriptionType,
            DateTime startDate,
            DateTime expiredDate
        )
        {
            SubscriptionType = subscriptionType;
            StartDate = startDate;
            ExpiredDate = expiredDate;
        }

        public static Result<Subscription> Create(
            int durationInMonth,
            long price,
            IDateTimeProvider dateTimeProvider
        )
        {
            var subscriptionType = SubscriptionType.Create(durationInMonth, price);
            if(subscriptionType.IsFailure)
            {
                return Result.Failure<Subscription>(subscriptionType.Error);
            }

            var startDate = dateTimeProvider.UtcNow;
            var expiredDate = startDate.AddMonths(durationInMonth);

            return new Subscription(subscriptionType.Value, startDate, expiredDate);
        }

        public bool IsExpired => ExpiredDate <= DateTime.UtcNow;
    }
}