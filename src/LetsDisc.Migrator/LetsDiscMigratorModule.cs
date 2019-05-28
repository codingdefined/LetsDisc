using Microsoft.Extensions.Configuration;
using Castle.MicroKernel.Registration;
using Abp.Events.Bus;
using Abp.Modules;
using Abp.Reflection.Extensions;
using LetsDisc.Configuration;
using LetsDisc.EntityFrameworkCore;
using LetsDisc.Migrator.DependencyInjection;

namespace LetsDisc.Migrator
{
    [DependsOn(typeof(LetsDiscEntityFrameworkModule))]
    public class LetsDiscMigratorModule : AbpModule
    {
        private readonly IConfigurationRoot _appConfiguration;

        public LetsDiscMigratorModule(LetsDiscEntityFrameworkModule abpProjectNameEntityFrameworkModule)
        {
            abpProjectNameEntityFrameworkModule.SkipDbSeed = false;

            _appConfiguration = AppConfigurations.Get(
                typeof(LetsDiscMigratorModule).GetAssembly().GetDirectoryPathOrNull()
            );
        }

        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = _appConfiguration.GetConnectionString(
                LetsDiscConsts.ConnectionStringName
            );

            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
            Configuration.ReplaceService(
                typeof(IEventBus), 
                () => IocManager.IocContainer.Register(
                    Component.For<IEventBus>().Instance(NullEventBus.Instance)
                )
            );
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(LetsDiscMigratorModule).GetAssembly());
            ServiceCollectionRegistrar.Register(IocManager);
        }
    }
}
