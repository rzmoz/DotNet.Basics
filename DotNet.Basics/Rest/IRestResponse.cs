using System;
using System.Net;
using System.Net.Http;

namespace DotNet.Basics.Rest
{
    public interface IRestResponse
    {
        HttpStatusCode StatusCode { get; }
        string ReasonPhrase { get; }
        string Body { get; }
        Uri Uri { get; }
        HttpResponseMessage HttpResponseMessage { get; }
    }
}
