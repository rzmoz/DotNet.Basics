using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace DotNet.Basics.Rest
{
    public interface IRestClient
    {
        HttpRequestHeaders DefaultRequestHeaders { get; }
        Uri BaseUri { get; set; }
        TimeSpan Timeout { get; set; }
        Task<IRestResponse> ExecuteAsync<T>(IRestRequest request, ResponseFormatting responseFormatting = ResponseFormatting.Raw);
        Task<IRestResponse> ExecuteAsync(IRestRequest request, ResponseFormatting responseFormatting = ResponseFormatting.Raw);
    }
}
