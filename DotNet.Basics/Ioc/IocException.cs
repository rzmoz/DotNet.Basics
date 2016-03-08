using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DotNet.Basics.Ioc
{
    [Serializable]
    public class IocException : Exception
    {
        public IocException()
        {
        }

        public IocException(string message)
            : base(message)
        {
        }

        public IocException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected IocException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
