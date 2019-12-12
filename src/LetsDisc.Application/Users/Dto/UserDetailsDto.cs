using System;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using LetsDisc.Authorization.Users;

namespace LetsDisc.Users.Dto
{
    [AutoMapFrom(typeof(UserDetails))]
    public class UserDetailsDto
    {
        public int Reputation { get; set; }
        public string DisplayName { get; set; }
        public string WebsiteUrl { get; set; }
        public string Location { get; set; }
        public string AboutMe { get; set; }
        public int Upvotes { get; set; }
        public int Downvotes { get; set; }
        public string Views { get; set; }
        public string ProfileImageUrl { get; set; }
        //public User User { get; set; }
        public long UserId { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
