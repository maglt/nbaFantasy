using System;
using System.Collections.Generic;
using System.Text;

namespace Fantasy
{
   public class User
    {
        public string UserName { get; set; }
        public string Email { get; set; }

        public User (string userName, string email)
        {
            this.UserName = userName;
            this.Email = email;
        }

        public Team CreateTeam(User fantasyUser,string teamName)
        {
            return new Team(fantasyUser , teamName);
        }


    }
}
