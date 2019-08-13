using System.Collections.Generic;
using Abp.Configuration;
using Abp.Net.Mail;
using LetsDisc.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace LetsDisc.Configuration
{
    public class AppSettingProvider : SettingProvider
    {
        private readonly IConfigurationRoot _appConfiguration;

        public AppSettingProvider(IHostingEnvironment env)
        {
            _appConfiguration = AppConfigurations.Get(env.ContentRootPath, env.EnvironmentName, env.IsDevelopment());
        }

        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            return new[]
            {
                new SettingDefinition(AppSettingNames.UiTheme, "red", scopes: SettingScopes.Application | SettingScopes.Tenant | SettingScopes.User, isVisibleToClients: true),
                new SettingDefinition(EmailSettingNames.DefaultFromAddress, "codingdefined@gmail.com"),
                new SettingDefinition(EmailSettingNames.DefaultFromDisplayName, "Gopesh Sharma"),
                new SettingDefinition(EmailSettingNames.Smtp.Host, "in-v3.mailjet.com"),
                new SettingDefinition(EmailSettingNames.Smtp.Port, "465"),
                new SettingDefinition(EmailSettingNames.Smtp.EnableSsl, "true"),
                new SettingDefinition(EmailSettingNames.Smtp.UseDefaultCredentials, "false"),
                new SettingDefinition(EmailSettingNames.Smtp.UserName, _appConfiguration["Authentication:Mailjet:UserName"]),
                new SettingDefinition(EmailSettingNames.Smtp.Password, _appConfiguration["Authentication:Mailjet:Password"])
            };
        }
    }
}
