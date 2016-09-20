using System;
using System.Globalization;
using DotNet.Basics.IO;

namespace DotNet.Basics.AppSettings
{
    public class AppSetting : AppSetting<string>
    {
        public AppSetting(string key) : base(key)
        { }

        public AppSetting(string key, bool required, string defaultValue) : base(key, required, defaultValue)
        { }

        public AppSetting(string key, IConfigurationManager configurationManager) : base(key, configurationManager)
        { }

        public AppSetting(string key, bool required, string defaultValue, IConfigurationManager configurationManager) : base(key, required, defaultValue, configurationManager)
        { }
    }

    public class AppSetting<T> : IAppSetting
    {
        private readonly IConfigurationManager _configurationManager;
        private readonly Func<object, object> _parser;
        private readonly IFormatProvider _usCulture = new CultureInfo("en-us");

        public AppSetting(string key)
            : this(key, new SystemConfigurationManager())
        { }

        /***********************************************************************/
        public AppSetting(string key, Func<string, T> customParser)
            : this(key, customParser, new SystemConfigurationManager())
        { }

        public AppSetting(string key, IConfigurationManager configurationManager)
            : this(key, true, default(T), configurationManager)
        { }

        public AppSetting(string key, bool required, T defaultValue)
            : this(key, required, defaultValue, new SystemConfigurationManager())
        { }

        /***********************************************************************/
        public AppSetting(string key, Func<string, T> customParser, IConfigurationManager configurationManager)
            : this(key, true, default(T), customParser, configurationManager)
        { }

        public AppSetting(string key, bool required, T defaultValue, Func<string, T> customParser)
            : this(key, required, defaultValue, customParser, new SystemConfigurationManager())
        { }

        public AppSetting(string key, bool required, T defaultValue, IConfigurationManager configurationManager)
            : this(key, required, defaultValue, null, configurationManager)
        { }

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
                _parser = value => customParser(value?.ToString());
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
                return (T)_parser(DefaultValue);

            throw new RequiredConfigurationNotSetException(Key);
        }

        private object DefaultParse(object value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            var parseType = typeof(T).FullName;
            switch (parseType)
            {
                case "DotNet.Basics.IO.DirPath":
                    return value.ToString().ToDir();
                case "DotNet.Basics.IO.FilePath":
                    return value.ToString().ToFile();
                case "System.String":
                    return value;
                case "System.Char":
                    return char.Parse(value.ToString());
                case "System.Int16":
                    return short.Parse(value.ToString(), _usCulture);
                case "System.UInt16":
                    return ushort.Parse(value.ToString(), _usCulture);
                case "System.Int32":
                    return int.Parse(value.ToString(), _usCulture);
                case "System.UInt32":
                    return uint.Parse(value.ToString(), _usCulture);
                case "System.Int64":
                    return long.Parse(value.ToString(), _usCulture);
                case "System.UInt64":
                    return ulong.Parse(value.ToString(), _usCulture);
                case "System.Boolean":
                    return bool.Parse(value.ToString());
                case "System.Double":
                    return double.Parse(value.ToString(), _usCulture);
                case "System.SByte":
                    return sbyte.Parse(value.ToString(), _usCulture);
                case "System.Byte":
                    return byte.Parse(value.ToString(), _usCulture);
                case "System.Decimal":
                    return decimal.Parse(value.ToString(), _usCulture);
                case "System.Single":
                    return float.Parse(value.ToString(), _usCulture);
                case "System.Guid":
                    return Guid.Parse(value.ToString());
                case "System.Uri":
                    return new Uri(value.ToString());
                default:
                    throw new NotSupportedException($"App setting type not supported: {parseType}");
            }
        }
    }
}
