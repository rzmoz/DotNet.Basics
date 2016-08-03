using System.Threading.Tasks;

namespace DotNet.Basics.Rest
{
    public interface IRestClient
    {
        Task<IRestResponse<T>> ExecuteAsync<T>(IRestRequest request, ResponseFormatting responseFormatting = ResponseFormatting.Raw);
        Task<IRestResponse> ExecuteAsync(IRestRequest request, ResponseFormatting responseFormatting = ResponseFormatting.Raw);
    }
}
