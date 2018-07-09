using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Abp.Localization;
using Abp.Application.Navigation;
using Abp.Runtime.Session;
using System.Threading.Tasks;
using LetsDisc.Sessions;
using Abp.Configuration.Startup;

namespace LetsDisc.Web.Views.Shared.Components.TopBarLanguageSwitch
{
    public class TopBarViewComponent : LetsDiscViewComponent
    {

        private readonly IUserNavigationManager _userNavigationManager;
        private readonly IAbpSession _abpSession;
        private readonly ISessionAppService _sessionAppService;
        private readonly IMultiTenancyConfig _multiTenancyConfig;

        public TopBarViewComponent(ISessionAppService sessionAppService,
            IMultiTenancyConfig multiTenancyConfig, IUserNavigationManager userNavigationManager,
            IAbpSession abpSession)
        {
            _sessionAppService = sessionAppService;
            _multiTenancyConfig = multiTenancyConfig;
            _userNavigationManager = userNavigationManager;
            _abpSession = abpSession;
        }

        public async Task<IViewComponentResult> InvokeAsync(string activeMenu = "")
        {
            var model = new TopBarViewModel
            {
                MainMenu = await _userNavigationManager.GetMenuAsync("MainMenu", _abpSession.ToUserIdentifier()),
                ActiveMenuItemName = activeMenu,
                LoginInformations = await _sessionAppService.GetCurrentLoginInformations(),
                IsMultiTenancyEnabled = _multiTenancyConfig.IsEnabled,
            };

            return View(model);
        }
    }
}
