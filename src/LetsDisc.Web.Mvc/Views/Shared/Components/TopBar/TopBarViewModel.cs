using System.Collections.Generic;
using Abp.Application.Navigation;
using Abp.Localization;
using LetsDisc.Sessions.Dto;
using Microsoft.AspNetCore.Authentication;

namespace LetsDisc.Web.Views.Shared.Components.TopBarLanguageSwitch
{
    public class TopBarViewModel
    {
        public GetCurrentLoginInformationsOutput LoginInformations { get; set; }

        public UserMenu MainMenu { get; set; }

        public string ActiveMenuItemName { get; set; }

        public bool IsMultiTenancyEnabled { get; set; }

        public string GetShownLoginName()
        {
            if(LoginInformations != null && LoginInformations.User != null)
            {
                var userName = "<a id=\"HeaderCurrentUserName\">" + LoginInformations.User.UserName + "</a>";

                if (!IsMultiTenancyEnabled)
                {
                    return userName;
                }

                return LoginInformations.Tenant == null
                    ? ".\\" + userName
                    : LoginInformations.Tenant.TenancyName + "\\" + userName;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
