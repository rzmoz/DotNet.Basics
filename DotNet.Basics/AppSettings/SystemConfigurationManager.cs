using System.Configuration;

namespace DotNet.Basics.AppSettings
{
    public class SystemConfigurationManager : IConfigurationManager
    {
        public string Get(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}
