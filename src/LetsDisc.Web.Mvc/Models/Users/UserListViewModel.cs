using System.Collections.Generic;
using LetsDisc.Roles.Dto;
using LetsDisc.Users.Dto;

namespace LetsDisc.Web.Models.Users
{
    public class UserListViewModel
    {
        public IReadOnlyList<UserDto> Users { get; set; }

        public IReadOnlyList<RoleDto> Roles { get; set; }
    }
}
