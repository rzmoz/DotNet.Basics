using System.Collections.Concurrent;
using System.Collections.Generic;

namespace DotNet.Basics.AppSettings
{
    public sealed class AppSettingsValidator
    {
        private readonly ConcurrentDictionary<string, IAppSetting> _settings;

        public AppSettingsValidator()
        {
            _settings = new ConcurrentDictionary<string, IAppSetting>();
        }

        public bool Register(IAppSetting setting)
        {
            return _settings.TryAdd(setting.Key, setting);
        }

        public IEnumerable<string> GetRequiredKeysNotSet()
        {
            foreach (var appSetting in _settings.Values)
            {
                if (appSetting.Verify() == false)
                    yield return appSetting.Key;
            }
        }
    }
}
