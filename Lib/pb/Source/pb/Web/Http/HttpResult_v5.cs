using System;
using System.Collections.Generic;

namespace pb.Web.Http
{
    public class HttpResult_v5
    {
        private bool _success = false;
        private int _statusCode;
        private bool _loadFromWeb = false;
        private bool _loadFromCache = false;
        private HttpMessageResult _httpMessageResult = null;
        private Func<HttpMessageResult> _getHttpMessageResult = null;

        public bool Success { get { return _success; } }
        public int StatusCode { get { return _statusCode; } }
        public bool LoadFromWeb { get { return _loadFromWeb; } }
        public bool LoadFromCache { get { return _loadFromCache; } }

        public HttpResult_v5(bool success, int statusCode, HttpMessageResult httpMessageResult, bool loadFromWeb = false, bool loadFromCache = false)
        {
            _success = success;
            _statusCode = statusCode;
            _httpMessageResult = httpMessageResult;
            _loadFromWeb = loadFromWeb;
            _loadFromCache = loadFromCache;
        }

        public HttpResult_v5(bool success, int statusCode, Func<HttpMessageResult> getHttpMessageResult, bool loadFromWeb = false, bool loadFromCache = false)
        {
            _success = success;
            _statusCode = statusCode;
            _getHttpMessageResult = getHttpMessageResult;
            _loadFromWeb = loadFromWeb;
            _loadFromCache = loadFromCache;
        }

        public HttpMessageResult GetHttpMessageResult()
        {
            if (_httpMessageResult == null)
                _httpMessageResult = _getHttpMessageResult();
            return _httpMessageResult;
        }
    }

    public class HttpResult_v5<T> : HttpResult_v5
    {
        public T Data;

        public HttpResult_v5(bool success, int statusCode, HttpMessageResult httpMessageResult, bool loadFromWeb = false, bool loadFromCache = false)
            : base(success, statusCode, httpMessageResult, loadFromWeb, loadFromCache)
        {
        }

        public HttpResult_v5(bool success, int statusCode, Func<HttpMessageResult> getHttpMessageResult, bool loadFromWeb = false, bool loadFromCache = false)
            : base(success, statusCode, getHttpMessageResult, loadFromWeb, loadFromCache)
        {
        }
    }

    public class HttpMessageResult
    {
        public DateTime RequestTime;
        public TimeSpan RequestDuration;
        public string CacheFile = null;      // sub-path of cache file
        public HttpRequestMessage Request;
        public HttpResponseMessage Response;

        public HttpMessageResult(System.Net.Http.HttpResponseMessage response, DateTime requestTime, TimeSpan requestDuration, string requestContent = null)
        {
            RequestTime = requestTime;
            RequestDuration = requestDuration;
            Request = new HttpRequestMessage(response.RequestMessage, requestContent);
            Response = new HttpResponseMessage(response);
        }
    }

    public class HttpRequestMessage
    {
        public Uri Uri;
        public string Method;
        public Dictionary<string, string> Headers;
        public string Content;

        public HttpRequestMessage(System.Net.Http.HttpRequestMessage request, string requestContent = null)
        {
            Uri = request.RequestUri;
            Method = request.Method.ToString();
            Headers = HttpTools.GetRequestHeaders(request);
            Content = requestContent;
        }
    }

    public class HttpResponseMessage
    {
        public int StatusCode;
        public string StatusMessage;
        public string Version;
        public string CharSet;
        public string MediaType;
        public Dictionary<string, string> Headers;

        public HttpResponseMessage(System.Net.Http.HttpResponseMessage response)
        {
            StatusCode = (int)response.StatusCode;
            StatusMessage = response.ReasonPhrase;
            Version = response.Version.ToString();
            Headers = HttpTools.GetResponseHeaders(response);
            CharSet = response.Content.Headers.ContentType?.CharSet;
            MediaType = response.Content.Headers.ContentType?.MediaType;
        }
    }
}
