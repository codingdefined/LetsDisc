using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using LetsDisc.Configuration;

namespace LetsDisc.Web.Host.Startup
{
    [DependsOn(
       typeof(LetsDiscWebCoreModule))]
    public class LetsDiscWebHostModule: AbpModule
    {
        private readonly IHostingEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public LetsDiscWebHostModule(IHostingEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(LetsDiscWebHostModule).GetAssembly());
        }
    }
}
