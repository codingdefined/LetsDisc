using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Runtime.Session;
using LetsDisc.Configuration.Dto;

namespace LetsDisc.Configuration
{
    [AbpAuthorize]
    public class ConfigurationAppService : LetsDiscAppServiceBase, IConfigurationAppService
    {
        public async Task ChangeUiTheme(ChangeUiThemeInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
        }
    }
}
