using System;
using System.Runtime.Serialization;

namespace DotNet.Basics.DependencyInjection
{
    public class TypeFailedToResolveException : Exception
    {
        public TypeFailedToResolveException(Type unresolvedType)
        {
            UnresolvedType = unresolvedType;
        }

        protected TypeFailedToResolveException(SerializationInfo info, StreamingContext context, Type unresolvedType) : base(info, context)
        {
            UnresolvedType = unresolvedType;
        }

        public TypeFailedToResolveException(string message, Type unresolvedType) : base(message)
        {
            UnresolvedType = unresolvedType;
        }

        public TypeFailedToResolveException(string message, Exception innerException, Type unresolvedType) : base(message, innerException)
        {
            UnresolvedType = unresolvedType;
        }

        public Type UnresolvedType { get; }
    }
}
