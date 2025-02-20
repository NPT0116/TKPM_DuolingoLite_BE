using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharedKernel;

namespace Domain.Entities.Subscriptions
{
    public class SubscriptionType : Entity
    {
        public int DurationInMonth { get; private set; }
        public long Price { get; private set; }

        private SubscriptionType() {}

        private SubscriptionType(
            int durationInMonth,
            long price
        )
        {
            DurationInMonth = durationInMonth;
            Price = price;
        }

        public static Result<SubscriptionType> Create(
            int durationInMonth,
            long price
        )
        {
            if(durationInMonth < 0)
            {
                return Result.Failure<SubscriptionType>(SubscriptionError.InvalidSubscriptionMonth(durationInMonth));
            }

            if(price < 0)
            {
                return Result.Failure<SubscriptionType>(SubscriptionError.NegativePrice(price));
            }

            var subscriptionType = new SubscriptionType(durationInMonth, price);

            return subscriptionType;
        }

    }
}