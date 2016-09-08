using System;

namespace DotNet.Basics.AppSettings
{
    public class AppSetting : AppSetting<string>
    {
        public AppSetting(string key) : base(key)
        {
        }

        public AppSetting(string key, bool required, string defaultValue) : base(key, required, defaultValue)
        {
        }

        public AppSetting(string key, IAppSettingsProvider appSettingsProvider) : base(key, appSettingsProvider)
        {
        }

        public AppSetting(string key, bool required, string defaultValue, IAppSettingsProvider appSettingsProvider) : base(key, required, defaultValue, appSettingsProvider)
        {
        }
    }
    public class AppSetting<T> : IAppSetting
    {
        private readonly IAppSettingsProvider _appSettingsProvider;
        private readonly Func<string, T> _customParser;

        public AppSetting(string key)
            : this(key, new ConfigurationManagerAppSettingsProvider())
        {
        }

        /***********************************************************************/
        public AppSetting(string key, Func<string, T> customParser)
            : this(key, customParser, new ConfigurationManagerAppSettingsProvider())
        {
        }
        public AppSetting(string key, IAppSettingsProvider appSettingsProvider)
            : this(key, true, default(T), appSettingsProvider)
        {
        }
        public AppSetting(string key, bool required, T defaultValue)
            : this(key, required, defaultValue, new ConfigurationManagerAppSettingsProvider())
        {
        }
        /***********************************************************************/
        public AppSetting(string key, Func<string, T> customParser, IAppSettingsProvider appSettingsProvider)
            : this(key, true, default(T), customParser, appSettingsProvider)
        {
        }
        public AppSetting(string key, bool required, T defaultValue, Func<string, T> customParser)
            : this(key, required, defaultValue, customParser, new ConfigurationManagerAppSettingsProvider())
        {
        }

        public AppSetting(string key, bool required, T defaultValue, IAppSettingsProvider appSettingsProvider)
            : this(key, required, defaultValue, null, appSettingsProvider)
        {
        }
        /***********************************************************************/
        public AppSetting(string key, bool required, T defaultValue, Func<string, T> customParser, IAppSettingsProvider appSettingsProvider)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (appSettingsProvider == null) throw new ArgumentNullException(nameof(appSettingsProvider));
            Key = key;
            Required = required;
            DefaultValue = defaultValue;
            _appSettingsProvider = appSettingsProvider;
            _customParser = customParser;
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

        public virtual T GetValue()
        {
            var value = _appSettingsProvider.Get(Key);
            if (value != null)
                return (T)Parse(value);

            if (Required == false)
                return (T)Parse(DefaultValue?.ToString());

            throw new RequiredAppSettingNotFoundException(Key);
        }

        private object Parse(string value)
        {
            if (_customParser != null)
                return _customParser(value);

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
                case "System.Uri":
                    return new Uri(value);
                default:
                    throw new NotSupportedException($"App setting type not supported: {parseType}");
            }
        }
    }
}
