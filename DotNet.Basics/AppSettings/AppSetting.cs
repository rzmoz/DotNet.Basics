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

        public AppSetting(string key, IConfigurationManager configurationManager) : base(key, configurationManager)
        {
        }

        public AppSetting(string key, bool required, string defaultValue, IConfigurationManager configurationManager) : base(key, required, defaultValue, configurationManager)
        {
        }
    }
    public class AppSetting<T> : IAppSetting
    {
        private readonly IConfigurationManager _configurationManager;
        private readonly Func<string, object> _parser;

        public AppSetting(string key)
            : this(key, new SystemConfigurationManager())
        {
        }

        /***********************************************************************/
        public AppSetting(string key, Func<string, T> customParser)
            : this(key, customParser, new SystemConfigurationManager())
        {
        }
        public AppSetting(string key, IConfigurationManager configurationManager)
            : this(key, true, default(T), configurationManager)
        {
        }
        public AppSetting(string key, bool required, T defaultValue)
            : this(key, required, defaultValue, new SystemConfigurationManager())
        {
        }
        /***********************************************************************/
        public AppSetting(string key, Func<string, T> customParser, IConfigurationManager configurationManager)
            : this(key, true, default(T), customParser, configurationManager)
        {
        }
        public AppSetting(string key, bool required, T defaultValue, Func<string, T> customParser)
            : this(key, required, defaultValue, customParser, new SystemConfigurationManager())
        {
        }

        public AppSetting(string key, bool required, T defaultValue, IConfigurationManager configurationManager)
            : this(key, required, defaultValue, null, configurationManager)
        {
        }
        /***********************************************************************/
        public AppSetting(string key, bool required, T defaultValue, Func<string, T> customParser, IConfigurationManager configurationManager)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (configurationManager == null) throw new ArgumentNullException(nameof(configurationManager));
            Key = key;
            Required = required;
            DefaultValue = defaultValue;
            _configurationManager = configurationManager;
            if (customParser == null)
                _parser = DefaultParse;
            else
                _parser = value => customParser(value);
        }


        public string Key { get; }
        public bool Required { get; }
        public T DefaultValue { get; }

        public bool Verify()
        {
            var value = _configurationManager.Get(Key);
            if (value != null)
                return true;

            return Required == false;
        }

        public virtual T GetValue()
        {
            var value = _configurationManager.Get(Key);
            if (value != null)
                return (T)_parser(value);

            if (Required == false)
                return (T)_parser(DefaultValue?.ToString());

            throw new RequiredConfigurationNotSetException(Key);
        }

        private object DefaultParse(object value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            var parseType = value.GetType().FullName;
            switch (parseType)
            {
                case "System.String":
                    return value;
                case "System.Int16":
                    return short.Parse(value.ToString());
                case "System.UInt16":
                    return ushort.Parse(value.ToString());
                case "System.Int32":
                    return int.Parse(value.ToString());
                case "System.UInt32":
                    return uint.Parse(value.ToString());
                case "System.Int64":
                    return long.Parse(value.ToString());
                case "System.UInt64":
                    return ulong.Parse(value.ToString());
                case "System.Boolean":
                    return bool.Parse(value.ToString());
                case "System.Double":
                    return double.Parse(value.ToString());
                case "System.Byte":
                    return byte.Parse(value.ToString());
                case "System.Decimal":
                    return decimal.Parse(value.ToString());
                case "System.Single":
                    return float.Parse(value.ToString());
                case "System.Uri":
                    return new Uri(value.ToString());
                default:
                    throw new NotSupportedException($"App setting type not supported: {parseType}");
            }
        }
    }
}
