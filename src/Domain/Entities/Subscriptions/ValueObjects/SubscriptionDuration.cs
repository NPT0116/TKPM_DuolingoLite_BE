using SharedKernel;

namespace Domain.Entities.Subscriptions.ValueObjects
{
    public record SubscriptionDuration
    {
        public static readonly SubscriptionDuration Monthly = new(1, "Monthly");
        public static readonly SubscriptionDuration Quarterly = new(3, "Quarterly");
        public static readonly SubscriptionDuration HalfYearly = new(6, "Half Yearly");
        public static readonly SubscriptionDuration Yearly = new(12, "Yearly");

        public int Months { get; }
        public string Name { get; }

        private SubscriptionDuration(int months, string name)
        {
            Months = months;
            Name = name;
        }

        public static IEnumerable<SubscriptionDuration> List() =>
            new[] { Monthly, Quarterly, HalfYearly, Yearly };

        public static SubscriptionDuration FromMonths(int months)
        {
            return List().FirstOrDefault(d => d.Months == months) 
                ?? throw new ArgumentException($"Duration of {months} months is not supported");
        }

        public DateTime CalculateExpiryDate(DateTime startDate) =>
            startDate.AddMonths(Months);
    }

    public static class SubscriptionDurationExtensions
    {
        public static Result<int> ToDays(this SubscriptionDuration duration)
        {
            if (duration == SubscriptionDuration.Monthly)
                return Result.Success(30);
            if (duration == SubscriptionDuration.Quarterly)
                return Result.Success(90);
            if (duration == SubscriptionDuration.HalfYearly)
                return Result.Success(180);
            if (duration == SubscriptionDuration.Yearly)
                return Result.Success(365);
            
            return Result.Failure<int>(SubscriptionError.InvalidSubscriptionDuration(duration));
        }

        public static Result<DateTime> CalculateExpiryDate(this SubscriptionDuration duration, DateTime startDate)
        {
            if (duration == SubscriptionDuration.Monthly)
                return Result.Success(startDate.AddMonths(1));
            if (duration == SubscriptionDuration.Quarterly)
                return Result.Success(startDate.AddMonths(3));
            if (duration == SubscriptionDuration.HalfYearly)
                return Result.Success(startDate.AddMonths(6));
            if (duration == SubscriptionDuration.Yearly)
                return Result.Success(startDate.AddYears(1));
            
            return Result.Failure<DateTime>(SubscriptionError.InvalidSubscriptionDuration(duration));
        }
    }
}