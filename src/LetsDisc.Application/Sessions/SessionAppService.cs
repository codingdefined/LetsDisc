using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Abp.Auditing;
using Abp.Runtime.Security;
using LetsDisc.Sessions.Dto;
using LetsDisc.SignalR;
using Microsoft.AspNetCore.Http;

namespace LetsDisc.Sessions
{
    public class SessionAppService : LetsDiscAppServiceBase, ISessionAppService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        protected SessionAppService()
        {
        }

        protected SessionAppService(HttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        [DisableAuditing]
        public async Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations()
        {
            var output = new GetCurrentLoginInformationsOutput
            {
                Application = new ApplicationInfoDto
                {
                    Version = AppVersionHelper.Version,
                    ReleaseDate = AppVersionHelper.ReleaseDate,
                    Features = new Dictionary<string, bool>
                    {
                        { "SignalR", SignalRFeature.IsAvailable },
                        { "SignalR.AspNetCore", SignalRFeature.IsAspNetCore }
                    }
                }
            };

            if (AbpSession.TenantId.HasValue)
            {
                output.Tenant = ObjectMapper.Map<TenantLoginInfoDto>(await GetCurrentTenantAsync());
            }

            if (AbpSession.UserId.HasValue)
            {
                output.User = ObjectMapper.Map<UserLoginInfoDto>(await GetCurrentUserAsync());
            } 
            else
            {
                var principal = Thread.CurrentPrincipal as ClaimsPrincipal;
                if (principal != null)
                {
                    var email = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
                    output.User = ObjectMapper.Map<UserLoginInfoDto>(await UserManager.FindByEmailAsync(email));
                }
            }
             
            return output;
        }
    }
}
