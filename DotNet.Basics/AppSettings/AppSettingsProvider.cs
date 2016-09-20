using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

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

        public bool VerifyAll()
        {
            IReadOnlyCollection<string> requiredKeysNotSet;
            return VerifyAll(out requiredKeysNotSet);
        }
        public bool VerifyAll(out IReadOnlyCollection<string> requiredKeysNotSet)

        {
            var missingKeys = new List<string>();
            foreach (var appSetting in _settings.Values)
            {
                if (appSetting.Verify() == false)
                    missingKeys.Add(appSetting.Key);
            }

            requiredKeysNotSet = missingKeys;

            return missingKeys.Count == 0;
        }

        public IReadOnlyCollection<IAppSetting> GetAppSettings()
        {
            return _settings.Values.ToList();
        }
    }
}
