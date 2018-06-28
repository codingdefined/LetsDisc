using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using LetsDisc.Authorization;

namespace LetsDisc
{
    [DependsOn(
        typeof(LetsDiscCoreModule), 
        typeof(AbpAutoMapperModule))]
    public class LetsDiscApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<LetsDiscAuthorizationProvider>();
        }

        public override void Initialize()
        {
            var thisAssembly = typeof(LetsDiscApplicationModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(thisAssembly);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                // Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg => cfg.AddProfiles(thisAssembly)
            );
        }
    }
}
