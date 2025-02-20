using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Media;
using Domain.Entities.Subscriptions;
using SharedKernel;

namespace Domain.Entities.Users
{
    public class UserProfile : Entity
    {
        public string? Email { get; private set; }
        public string NickName { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public Media.Media? ProfileImage { get; private set; }
        public Subscription? Subscription { get; private set; }

        private UserProfile() { }

        public UserProfile(string email, string nickName, string firstName, string lastName, Media.Media? profileImage, Subscription? subscription)
        {
            Email = email;
            NickName = nickName;
            FirstName = firstName;
            LastName = lastName;
            ProfileImage = profileImage;
            Subscription = subscription;
        }

        public void UpdateNickName(string newNickName) => NickName = newNickName;
    }

}