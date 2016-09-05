using System;

namespace DotNet.Basics.AppSettings
{
    public class AppSetting<T>
    {
        private readonly IAppSettingsProvider _appSettingsProvider;

        public AppSetting(string key)
            : this(key, new ConfigurationManagerAppSettingsProvider())
        {
        }

        public AppSetting(string key, bool required, T defaultValue)
            : this(key, required, defaultValue, new ConfigurationManagerAppSettingsProvider())
        {
        }

        public AppSetting(string key, IAppSettingsProvider appSettingsProvider)
            : this(key, true, default(T), appSettingsProvider)
        {
        }

        public AppSetting(string key, bool required, T defaultValue, IAppSettingsProvider appSettingsProvider)
        {

            if (key == null) throw new ArgumentNullException(nameof(key));
            if (appSettingsProvider == null) throw new ArgumentNullException(nameof(appSettingsProvider));
            Key = key;
            Required = required;
            DefaultValue = defaultValue;
            _appSettingsProvider = appSettingsProvider;
        }

        public string Key { get; }
        public bool Required { get; }
        public T DefaultValue { get; }

        public bool Verify()
        {
            var value = _appSettingsProvider.Get(Key);
            if (value != null)
                return true;

            return Required == false;
        }

        public T GetValue()
        {
            var value = _appSettingsProvider.Get(Key);
            if (value != null)
                return (T)Parse(value);

            if (Required == false)
                return (T)Parse(DefaultValue?.ToString());

            throw new RequiredAppSettingFoundException(Key);
        }

        private object Parse(string value)
        {
            var parseType = typeof(T).FullName;

            switch (parseType)
            {
                case "System.String":
                    return value;
                case "System.Int16":
                    return short.Parse(value);
                case "System.UInt16":
                    return ushort.Parse(value);
                case "System.Int32":
                    return int.Parse(value);
                case "System.UInt32":
                    return uint.Parse(value);
                case "System.Int64":
                    return long.Parse(value);
                case "System.UInt64":
                    return ulong.Parse(value);
                case "System.Boolean":
                    return bool.Parse(value);
                case "System.Double":
                    return double.Parse(value);
                case "System.Byte":
                    return byte.Parse(value);
                case "System.Decimal":
                    return decimal.Parse(value);
                case "System.Single":
                    return float.Parse(value);
                default:
                    throw new NotSupportedException($"App setting type not supported: {parseType}");
            }
        }
    }
}
