using Abp.AspNetCore.Mvc.ViewComponents;

namespace LetsDisc.Web.Views
{
    public abstract class LetsDiscViewComponent : AbpViewComponent
    {
        protected LetsDiscViewComponent()
        {
            LocalizationSourceName = LetsDiscConsts.LocalizationSourceName;
        }
    }
}
