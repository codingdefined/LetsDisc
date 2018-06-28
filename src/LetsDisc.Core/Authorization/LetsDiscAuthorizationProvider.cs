using Abp.Authorization;
using Abp.Localization;
using Abp.MultiTenancy;

namespace LetsDisc.Authorization
{
    public class LetsDiscAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            context.CreatePermission(PermissionNames.Pages_Users, L("Users"));
            context.CreatePermission(PermissionNames.Pages_Roles, L("Roles"));

            // Giving a Role that the User Can Create Questions
            context.CreatePermission(PermissionNames.Pages_Questions_Create, L("Can_create_questions"));
            context.CreatePermission(PermissionNames.Pages_Tenants, L("Tenants"), multiTenancySides: MultiTenancySides.Host);
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, LetsDiscConsts.LocalizationSourceName);
        }
    }
}
