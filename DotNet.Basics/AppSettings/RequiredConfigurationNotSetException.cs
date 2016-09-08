using System;
using System.Runtime.Serialization;

namespace DotNet.Basics.AppSettings
{
    public class RequiredConfigurationNotSetException : Exception
    {
        public RequiredConfigurationNotSetException(string message) : base(message)
        {
        }

        public RequiredConfigurationNotSetException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RequiredConfigurationNotSetException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
