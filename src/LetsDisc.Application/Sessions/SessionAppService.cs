using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Auditing;
using LetsDisc.Sessions.Dto;
using LetsDisc.SignalR;

namespace LetsDisc.Sessions
{
    public class SessionAppService : LetsDiscAppServiceBase, ISessionAppService
    {
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
             
            return output;
        }
    }
}
