using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    }
}