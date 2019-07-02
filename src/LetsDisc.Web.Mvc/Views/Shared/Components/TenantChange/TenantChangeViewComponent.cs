﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Abp.AutoMapper;
using LetsDisc.Sessions;

namespace LetsDisc.Web.Views.Shared.Components.TenantChange
{
    public class TenantChangeViewComponent : LetsDiscViewComponent
    {
        private readonly ISessionAppService _sessionAppService;

        public TenantChangeViewComponent(ISessionAppService sessionAppService)
        {
            _sessionAppService = sessionAppService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var loginInfo = await _sessionAppService.GetCurrentLoginInformations("admin@aspnetboilerplate.com");
            var model = loginInfo.MapTo<TenantChangeViewModel>();
            return View(model);
        }
    }
}
