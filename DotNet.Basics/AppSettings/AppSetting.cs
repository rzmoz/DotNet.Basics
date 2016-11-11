using System;
using System.Globalization;

namespace DotNet.Basics.AppSettings
{
    public class AppSetting : AppSetting<string>
    {
        public AppSetting(string key, Func<string, string> customParser = null, IConfigurationManager configurationManager = null) : base(key, customParser, configurationManager)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue">Setting default value will cause the settings validator to return this value instead of throwing exception if key is not set in configuration</param>
        /// <param name="customParser"></param>
        /// <param name="configurationManager"></param>
        public AppSetting(string key, string defaultValue, Func<string, string> customParser = null, IConfigurationManager configurationManager = null) : base(key, defaultValue, customParser, configurationManager)
        { }
    }

    public class AppSetting<T> : IAppSetting
    {
        private readonly IConfigurationManager _configurationManager;
        private readonly Func<object, object> _parser;
        private readonly IFormatProvider _usCulture = new CultureInfo("en-us");
        
        public AppSetting(string key, Func<string, T> customParser = null,
            IConfigurationManager configurationManager = null)
            : this(key, true, default(T), customParser, configurationManager)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue">Setting default value will cause the settings validator to return this value instead of throwing exception if key is not set in configuration</param>
        /// <param name="customParser"></param>
        /// <param name="configurationManager"></param>
        public AppSetting(string key, T defaultValue, Func<string, T> customParser = null, IConfigurationManager configurationManager = null)
            : this(key, false, defaultValue, customParser, configurationManager)
        { }

        private AppSetting(string key, bool required, T defaultValue, Func<string, T> customParser = null,
            IConfigurationManager configurationManager = null)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            Key = key;
            DefaultValue = defaultValue;
            Required = required;
            _configurationManager = configurationManager ?? new SystemConfigurationManager();
            if (customParser == null)
                _parser = DefaultParse;
            else
                _parser = value => customParser(value?.ToString());
        }

        public string Key { get; }
        public T DefaultValue { get; set; }
        public bool Required { get; }

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

            throw new RequiredConfigurationKeyNotSetException(Key);
        }

        private object DefaultParse(object value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            var parseType = typeof(T).FullName;
            switch (parseType)
            {
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
