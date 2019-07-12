using System;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using LetsDisc.Authorization.Users;

namespace LetsDisc.Users.Dto
{
    public class UserDetailsDto
    {
        public int Reputation { get; set; }
        public string DisplayName { get; set; }
        public string WebsiteUrl { get; set; }
        public string Location { get; set; }
        public string AboutMe { get; set; }
        public string Views { get; set; }
        public string ProfileImageUrl { get; set; }
    }
}
