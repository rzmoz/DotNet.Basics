using System.Configuration;

namespace DotNet.Basics.AppSettings
{
    public class ConfigurationManagerAppSettingsProvider : IAppSettingsProvider
    {
        public string Get(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}
