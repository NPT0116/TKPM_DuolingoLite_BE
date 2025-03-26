using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Subscriptions.ValueObjects;
using SharedKernel;

namespace Domain.Entities.Subscriptions
{
    public class SubscriptionError
    {
        public static Error InvalidSubscriptionDuration(SubscriptionDuration duration) => Error.Validation(
            "Subscription.InvalidSubscriptionDuration",
            "Invalid subscription duration"
        );

        public static Error InvalidSubscriptionMonth(int month) => Error.Validation(
            "Subscription.InvalidSubscriptionMonth",
            $"Number of months {month} is negative"  
        );

        public static Error NegativePrice(long price) => Error.Validation(
             "Subscription.NegativePrice",
            $"Price {price} is negative"  
        );
        public static Error AlreadyInSubscription() => Error.Validation(
            "Subscription.AlreadyInSubscription",
            "User is already in subscription"
        );
    }
}