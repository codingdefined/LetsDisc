using Microsoft.AspNetCore.Mvc;
using Abp.AspNetCore.Mvc.Authorization;
using LetsDisc.Controllers;

namespace LetsDisc.Web.Controllers
{
    public class AboutController : LetsDiscControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }
	}
}
