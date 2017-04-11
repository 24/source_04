using System;
using System.Collections.Generic;

namespace pb.Web.Http
{
    public class HttpResult_v3
    {
        private bool _success = false;
        private bool _loadFromWeb = false;
        private bool _loadFromCache = false;
        private HttpMessageResult _httpMessageResult = null;
        //private Http_v3 _http = null;
        private Func<HttpMessageResult> _getHttpMessageResult = null;

        //public HttpRequest_v3 HttpRequest = null;
        public bool Success { get { return _success; } }
        public bool LoadFromWeb { get { return _loadFromWeb; } }
        public bool LoadFromCache { get { return _loadFromCache; } }

        public HttpResult_v3(bool success, HttpMessageResult httpMessageResult, bool loadFromWeb = false, bool loadFromCache = false)
        {
            _success = success;
            _httpMessageResult = httpMessageResult;
            _loadFromWeb = loadFromWeb;
            _loadFromCache = loadFromCache;
        }

        public HttpResult_v3(bool success, Func<HttpMessageResult> getHttpMessageResult, bool loadFromWeb = false, bool loadFromCache = false)
        {
            _success = success;
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

    public class HttpResult_v3<T> : HttpResult_v3
    {
        public T Data;

        public HttpResult_v3(bool success, HttpMessageResult httpMessageResult, bool loadFromWeb = false, bool loadFromCache = false) : base(success, httpMessageResult, loadFromWeb, loadFromCache)
        {
        }

        public HttpResult_v3(bool success, Func<HttpMessageResult> getHttpMessageResult, bool loadFromWeb = false, bool loadFromCache = false) : base(success, getHttpMessageResult, loadFromWeb, loadFromCache)
        {
        }
    }

    // HttpLog_v3
    public class HttpMessageResult
    {
        public DateTime RequestTime;
        public TimeSpan RequestDuration;
        //public string Encoding;
        public string CacheFile = null;      // sub-path of cache file
        public HttpRequestMessage Request;
        public HttpResponseMessage Response;
        //public HttpRequestResult Values;

        public HttpMessageResult(System.Net.Http.HttpResponseMessage response, DateTime requestTime, TimeSpan requestDuration, string requestContent = null)
        {
            RequestTime = requestTime;
            RequestDuration = requestDuration;
            Request = new HttpRequestMessage(response.RequestMessage, requestContent);
            Response = new HttpResponseMessage(response);
            //Values = new HttpRequestResult();
        }
    }

    // HttpRequestLog_v3
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

    // HttpResponseLog_v3
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

    // HttpValuesLog_v3
    //public class HttpRequestResult
    //{
    //    public DateTime RequestTime;
    //    public TimeSpan RequestDuration;
    //    public string Encoding;
    //    public string CacheFile = null;      // sub-path of cache file
    //}
}
