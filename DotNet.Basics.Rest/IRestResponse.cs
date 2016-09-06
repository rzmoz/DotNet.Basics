using System;
using System.Net;
using System.Net.Http;

namespace DotNet.Basics.Rest
{
    public interface IRestResponse<out T> : IRestResponse
    {
        T Content { get; }
    }

    public interface IRestResponse
    {
        HttpStatusCode StatusCode { get; }
        string ReasonPhrase { get; }
        string RawContent { get; }
        Uri Uri { get; }
        HttpResponseMessage HttpResponseMessage { get; }
    }
}
