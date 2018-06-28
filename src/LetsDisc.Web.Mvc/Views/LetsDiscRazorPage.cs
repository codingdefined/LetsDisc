using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Abp.AspNetCore.Mvc.Views;
using Abp.Runtime.Session;

namespace LetsDisc.Web.Views
{
    public abstract class LetsDiscRazorPage<TModel> : AbpRazorPage<TModel>
    {
        [RazorInject]
        public IAbpSession AbpSession { get; set; }

        protected LetsDiscRazorPage()
        {
            LocalizationSourceName = LetsDiscConsts.LocalizationSourceName;
        }
    }
}
