using Abp.Authorization;
using LetsDisc.Authorization.Roles;
using LetsDisc.Authorization.Users;

namespace LetsDisc.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {
        }
    }
}
