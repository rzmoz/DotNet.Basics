using System.Net.Http;
using System.Threading.Tasks;

namespace DotNet.Basics.Net
{
    public interface IHttpTransport
    {
        Task<HttpResponseMessage> SendRequestAsync(IRestRequest request);
    }
}