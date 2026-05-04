using System;
using System.ComponentModel;
using System.Globalization;

namespace DotNet.Basics.Sys.Text
{
    public sealed class FilePathTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) => sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if (value is string str)
                return str.ToFile();
            return base.ConvertFrom(context, culture, value);
        }
    }
}
