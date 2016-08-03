using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace DotNet.Basics.RestClient
{
    public interface IRestRequest
    {
        Uri Uri { get; }
        HttpMethod Method { get; set; }
        HttpRequestHeaders Headers { get; }
        HttpContent Content { get; }
        TimeSpan TimeOut { get; }
        HttpRequestMessage HttpRequestMessage { get; }
    }
}
