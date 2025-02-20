using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharedKernel;

namespace Domain.Entities.User.ValueObjects
{
    public class EmailError 
    {
        public static Error Empty => new Error("Email.Empty", "Email cannot be empty", ErrorType.Validation);
        public static Error InvalidFormat => new Error("Email.InvalidFormat", "Email is invalid", ErrorType.Validation);
        public static Error TooLong => new Error("Email.TooLong", "Email is too long", ErrorType.Validation);
    }
}