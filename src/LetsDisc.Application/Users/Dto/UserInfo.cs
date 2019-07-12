using System;
using System.Collections.Generic;
using System.Text;

namespace LetsDisc.Users.Dto
{
    public class UserInfo
    {
        public UserDto User { get; set; }
        public UserDetailsDto UserDetails { get; set; }
        public int questionsCount { get; set; }
        public int answersCount { get; set; }
    }
}
