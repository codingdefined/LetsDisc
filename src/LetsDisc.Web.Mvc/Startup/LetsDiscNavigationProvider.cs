using Abp.Application.Navigation;
using Abp.Localization;
using LetsDisc.Authorization;

namespace LetsDisc.Web.Startup
{
    /// <summary>
    /// This class defines menus for the application.
    /// </summary>
    public class LetsDiscNavigationProvider : NavigationProvider
    {
        public override void SetNavigation(INavigationProviderContext context)
        {
            context.Manager.MainMenu
                .AddItem(
                    new MenuItemDefinition(
                        PageNames.Questions,
                        L("Questions"),
                        url: "",
                        icon: "question_answer"
                    )
                )
                .AddItem(
                    new MenuItemDefinition(
                        PageNames.Users,
                        L("Users"),
                        url: "Users",
                        icon: "people",
                        requiresAuthentication: true
                    )
                )
                .AddItem(
                    new MenuItemDefinition(
                        PageNames.About,
                        L("About"),
                        url: "About",
                        icon: "info"
                    )
                );
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, LetsDiscConsts.LocalizationSourceName);
        }
    }
}
