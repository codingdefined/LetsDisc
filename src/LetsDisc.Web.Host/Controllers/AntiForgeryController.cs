using Microsoft.AspNetCore.Antiforgery;
using LetsDisc.Controllers;

namespace LetsDisc.Web.Host.Controllers
{
    public class AntiForgeryController : LetsDiscControllerBase
    {
        private readonly IAntiforgery _antiforgery;

        public AntiForgeryController(IAntiforgery antiforgery)
        {
            _antiforgery = antiforgery;
        }

        public void GetToken()
        {
            _antiforgery.SetCookieTokenAndHeader(HttpContext);
        }
    }
}
