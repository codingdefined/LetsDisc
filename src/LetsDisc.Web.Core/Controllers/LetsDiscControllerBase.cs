using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace LetsDisc.Controllers
{
    public abstract class LetsDiscControllerBase: AbpController
    {
        protected LetsDiscControllerBase()
        {
            LocalizationSourceName = LetsDiscConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
