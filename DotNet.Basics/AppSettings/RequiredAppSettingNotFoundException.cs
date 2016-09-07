using System;
using System.Runtime.Serialization;

namespace DotNet.Basics.AppSettings
{
    public class RequiredAppSettingNotFoundException : Exception
    {
        public RequiredAppSettingNotFoundException(string message) : base(message)
        {
        }

        public RequiredAppSettingNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RequiredAppSettingNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
