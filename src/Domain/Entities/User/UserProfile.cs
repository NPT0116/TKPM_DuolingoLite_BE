using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Media;
using SharedKernel;

namespace Domain.Entities.User
{
    public class UserProfile : Entity
    {
        public string NickName { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public Media.Media? ProfileImage { get; private set; }
        public Subscription.Subscription? Subscription { get; private set; }

        private UserProfile() { }

        public UserProfile(string nickName, string firstName, string lastName, Media.Media? profileImage, Subscription.Subscription? subscription)
        {
            NickName = nickName;
            FirstName = firstName;
            LastName = lastName;
            ProfileImage = profileImage;
            Subscription = subscription;
        }

        public void UpdateNickName(string newNickName) => NickName = newNickName;
    }

}