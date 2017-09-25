using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace DotNet.Basics.Rest
{
    public interface IRestRequest
    {
        Uri Uri { get; set; }
        HttpRequestHeaders Headers { get; }
        HttpMethod Method { get; set; }
        HttpContent Content { get; set; }
        TimeSpan TimeOut { get; }
        HttpRequestMessage HttpRequestMessage { get; }
    }
}
