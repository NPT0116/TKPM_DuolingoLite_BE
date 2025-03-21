using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Media;
using Domain.Entities.Subscriptions;
using Domain.Entities.User.ValueObjects;
using SharedKernel;

namespace Domain.Entities.Users
{
    public class UserProfile : Entity
    {
        public Guid UserId { get; private set; }
        public Email? Email { get; private set; }
        public string NickName { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public Media.Media? ProfileImage { get; private set; }
        public Subscription? Subscription { get; private set; }

        private UserProfile() { }

        public UserProfile(Guid userId, Email email, string nickName, string firstName, string lastName, Media.Media? profileImage, Subscription? subscription)
        {
            UserId = userId;
            Email = email;
            NickName = nickName;
            FirstName = firstName;
            LastName = lastName;
            ProfileImage = profileImage;
            Subscription = subscription;
        }

        public void UpdateNickName(string newNickName) => NickName = newNickName;

        public static Result<UserProfile> Create(Guid userId, string email, string nickName, string firstName, string lastName, Media.Media? profileImage, Subscription? subscription)
        {
            var emailResult = Email.Create(email);
            if (emailResult.IsFailure)
            {
                return Result.Failure<UserProfile>(emailResult.Error);
            }
            var userProfile = new UserProfile(userId, emailResult.Value, nickName, firstName, lastName, profileImage, subscription);
            return Result.Success(userProfile);
        }

        public void UpdateProfileImage(Media.Media newProfileImage)
        {
            ProfileImage = newProfileImage;
        }
    }

}