using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Users.Constants;
using SharedKernel;

namespace Domain.Entities.Users
{
    public class HeartError
    {
        public static Error CannotDeclineHeartWhenAtMinimum => Error.Validation(
        "Hearts.CannotDeclineHeartWhenAtMinimum",
        $"Can not decline heart when it reaches minimum value");

        public static Error CannotIncreaseHeartWhenAtMaximum => Error.Validation(
        "Hearts.CannotIncreaseHeartWhenAtMaximum",
        $"Can not increase heart when it reaches maximum value");

        public static Error OutOfRange => Error.Validation(
        "Hearts.OutOfRange",
        $"Heart must be in range {HeartConstants.MINIMUM_HEART} and {HeartConstants.MAXIMUM_HEART}");

        public static Error PremiumUserHeartDecreaseNotAllowedException => Error.Forbidden(
        "Hearts.PremiumUserHeartDecreaseNotAllowedException",
        $"Can not decrease heart of a premium user");
    }
}