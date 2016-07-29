﻿using System;
using System.Net;
using System.Net.Http;

namespace DotNet.Basics.Net
{
    public interface IRestResponse<out T> : IRestResponse
    {
        T Content { get; }
    }

    public interface IRestResponse
    {
        HttpStatusCode StatusCode { get; }
        string RawContent { get; }
        Uri Uri { get; }
        HttpResponseMessage HttpResponseMessage { get; }
    }
}
