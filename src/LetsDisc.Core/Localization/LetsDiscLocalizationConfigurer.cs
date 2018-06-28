using Abp.Configuration.Startup;
using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Xml;
using Abp.Reflection.Extensions;

namespace LetsDisc.Localization
{
    public static class LetsDiscLocalizationConfigurer
    {
        public static void Configure(ILocalizationConfiguration localizationConfiguration)
        {
            localizationConfiguration.Sources.Add(
                new DictionaryBasedLocalizationSource(LetsDiscConsts.LocalizationSourceName,
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                        typeof(LetsDiscLocalizationConfigurer).GetAssembly(),
                        "LetsDisc.Localization.SourceFiles"
                    )
                )
            );
        }
    }
}
