using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using SharedKernel;

namespace Domain.Entities.User.ValueObjects
{
    public class Email
    {
        public string Value { get; }

        private Email(string value)
        {
            Value = value;
        }

        public static Result<Email> Create(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return Result.Failure<Email>(EmailError.Empty);

            email = email.Trim();

            if (email.Length > 256)
                return Result.Failure<Email>(EmailError.TooLong);

            if (!IsValidEmail(email))
                return Result.Failure<Email>(EmailError.InvalidFormat);

            return new Email(email);
        }

        private static bool IsValidEmail(string email)
        {
            // RFC 5322 Standard Email Validation
            string pattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";
            
            return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
        }
        
        public static implicit operator string(Email email) => email.Value;

        public override string ToString() => Value;
    }
}