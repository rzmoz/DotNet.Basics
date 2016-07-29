using System;
using System.Net.Http;
using System.Runtime.Serialization;

namespace DotNet.Basics.Net
{
    public class RestRequestException : Exception
    {
        public RestRequestException(IRestRequest request, HttpResponseMessage response)
        {
            Request = request;
            Response = response;
        }

        public RestRequestException(string message, IRestRequest request, HttpResponseMessage response)
            : base(message)
        {
            Request = request;
            Response = response;
        }

        public RestRequestException(string message, Exception innerException, IRestRequest request, HttpResponseMessage response)
            : base(message, innerException)
        {
            Request = request;
            Response = response;
        }

        protected RestRequestException(SerializationInfo info, StreamingContext context, IRestRequest request, HttpResponseMessage response)
            : base(info, context)
        {
            Request = request;
            Response = response;
        }

        public IRestRequest Request { get; }
        public HttpResponseMessage Response { get; }
    }
}
