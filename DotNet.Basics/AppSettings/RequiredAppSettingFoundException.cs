using System;
using System.Runtime.Serialization;

namespace DotNet.Basics.AppSettings
{
    public class RequiredAppSettingFoundException : Exception
    {
        public RequiredAppSettingFoundException(string message) : base(message)
        {
        }

        public RequiredAppSettingFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RequiredAppSettingFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
