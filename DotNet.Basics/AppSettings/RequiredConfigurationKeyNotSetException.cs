using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DotNet.Basics.AppSettings
{
    public class RequiredConfigurationKeyNotSetException : Exception
    {
        public RequiredConfigurationKeyNotSetException(params string[] missingKeys)
        {
            MissingKeys = missingKeys;
        }

        public RequiredConfigurationKeyNotSetException(string message, IReadOnlyCollection<string> missingKeys) : base(message)
        {
            MissingKeys = missingKeys;
        }

        public RequiredConfigurationKeyNotSetException(string message, Exception innerException, IReadOnlyCollection<string> missingKeys) : base(message, innerException)
        {
            MissingKeys = missingKeys;
        }

        protected RequiredConfigurationKeyNotSetException(SerializationInfo info, StreamingContext context, IReadOnlyCollection<string> missingKeys) : base(info, context)
        {
            MissingKeys = missingKeys;
        }

        public IReadOnlyCollection<string> MissingKeys { get; }
    }
}
