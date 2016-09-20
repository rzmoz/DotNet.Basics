using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace DotNet.Basics.AppSettings
{
    public class AppSettingsProvider : IAppSettingsProvider
    {
        private readonly IDictionary<string, IAppSetting> _settings;

        public AppSettingsProvider()
        {
            _settings = new ConcurrentDictionary<string, IAppSetting>();
        }

        public void Register(IAppSetting setting)
        {
            _settings[setting.Key] = setting;
        }

        public void VerifyAll()
        {
            var missingKeys = new List<string>();
            foreach (var appSetting in _settings.Values)
            {
                if (appSetting.Verify() == false)
                    missingKeys.Add(appSetting.Key);
            }

            //all is good
            if (missingKeys.Count == 0)
                return;

            var errorMessage = missingKeys.Count == 1 ? missingKeys.Single() : JsonConvert.SerializeObject(missingKeys);

            throw new RequiredConfigurationNotSetException(errorMessage);
        }

        public IReadOnlyCollection<IAppSetting> GetAppSettings()
        {
            return _settings.Values.ToList();
        }
    }
}
