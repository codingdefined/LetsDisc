using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Abp.Application.Services;
using Abp.IdentityFramework;
using Abp.Runtime.Session;
using LetsDisc.Authorization.Users;
using LetsDisc.MultiTenancy;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace LetsDisc
{
    /// <summary>
    /// Derive your application services from this class.
    /// </summary>
    public abstract class LetsDiscAppServiceBase : ApplicationService
    {
        public TenantManager TenantManager { get; set; }

        public UserManager UserManager { get; set; }

        protected LetsDiscAppServiceBase()
        {
            LocalizationSourceName = LetsDiscConsts.LocalizationSourceName;
        }

        protected async virtual Task<User> GetCurrentUserAsync()
        {
            var user = await UserManager.FindByIdAsync(AbpSession.GetUserId().ToString());
            if (user == null)
            {
                throw new Exception("There is no current user!");
            }

            return user;
        }

        protected async virtual Task<User> GetUserByEmailAsync(string email)
        {
            var user = await UserManager.FindByEmailAsync(email);
            if (user == null)
            {
                throw new Exception("There is no current user!");
            }

            return user;
        }

        protected virtual Task<Tenant> GetCurrentTenantAsync()
        {
            return TenantManager.GetByIdAsync(AbpSession.GetTenantId());
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
