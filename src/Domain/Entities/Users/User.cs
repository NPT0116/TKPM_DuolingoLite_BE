using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Domain.Entities.Learning.LearningProgress;
using Domain.Entities.User.ValueObjects;
using SharedKernel;

namespace Domain.Entities.User
{
    public class User : Entity
    {
        public string NickName { get; private set; } = string.Empty;
        public Email Email { get; private set; }
        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;
        public DateTime JoinDate { get; private set; }
        public int ExperiencePoint { get; private set; }
        public int Heart { get; private set; }
        public int CurrentStreak { get; private set; }
        public int LongestStreak { get; private set; }
        public DateTime? LastActiveDate { get; private set; }
        public Media.Media? ProfileImage { get; private set; }
        public Subscription.Subscription? Subscription { get; private set; }
        private readonly List<LearningProgress> _learningProgresses = new();
        public IReadOnlyList<LearningProgress> LearningProgresses => _learningProgresses.AsReadOnly();
        private readonly List<UserActivity> _userActivities = new();
        public IReadOnlyList<UserActivity> UserActivities => _userActivities.AsReadOnly();
        
        private User() { }

        private User(
            string nickName, 
            string email,
            string firstName, 
            string lastName,
            IDateTimeProvider dateTimeProvider)
        {
            Email = Email.Create(email).Value;
            NickName = nickName;
            FirstName = firstName;
            LastName = lastName;
            JoinDate = dateTimeProvider.UtcNow;
            ExperiencePoint = 0;
            Heart = 5;
            CurrentStreak = 0;
            LongestStreak = 0;
            LastActiveDate = dateTimeProvider.UtcNow;
            ProfileImage = null;
            Subscription = null;
        }

        public static Result<User> Create(
            string nickName, 
            string email, 
            string firstName, 
            string lastName,
            IDateTimeProvider dateTimeProvider)
        {
            var emailResult = Email.Create(email);
            if (emailResult.IsFailure)
                return Result.Failure<User>(emailResult.Error);

            return new User(nickName, emailResult.Value, firstName, lastName, dateTimeProvider);
        }
    }
}