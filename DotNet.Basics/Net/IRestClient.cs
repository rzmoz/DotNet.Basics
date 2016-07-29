﻿using System.Threading.Tasks;

namespace DotNet.Basics.Net
{
    public interface IRestClient
    {
        Task<IRestResponse<T>> ExecuteAsync<T>(IRestRequest request, ResponseFormatting responseFormatting = ResponseFormatting.Raw);
        Task<IRestResponse> ExecuteAsync(IRestRequest request, ResponseFormatting responseFormatting = ResponseFormatting.Raw);
    }
}
