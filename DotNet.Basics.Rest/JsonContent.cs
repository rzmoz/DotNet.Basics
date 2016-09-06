﻿using System.Net.Http;
using System.Text;

namespace DotNet.Basics.Rest
{
    public class JsonContent : StringContent
    {
        private const string _cdefaultContentType = "application/json";

        public JsonContent(string json) : this(json, Encoding.UTF8)
        {
        }

        public JsonContent(string json, Encoding encoding) : this(json, encoding, _cdefaultContentType)
        {
        }

        public JsonContent(string json, Encoding encoding, string mediaType) : base(json, encoding, mediaType)
        {
        }
    }
}