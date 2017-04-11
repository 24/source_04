using pb.IO;
using System;
using System.IO;
using System.Threading.Tasks;

namespace pb.Web.Http
{
    public class HttpRequest_v3 : IDisposable
    {
        public string Url;
        public HttpRequestMethod Method = HttpRequestMethod.Get;
        public string Referer = null;
        public HttpContentValue Content = null;
        public bool ReloadFromWeb = false;
        // used in Http_v3
        public string CacheSubDirectory = null;
        //public UrlCachePathResult UrlCachePath = null;

        public virtual void Dispose()
        {
            if (Content != null)
                Content.Dispose();
        }
    }

    public class Http_v3 : IDisposable
    {
        // http request
        private HttpRequest_v3 _request = null;
        //private HttpRequestParameters _requestParameters = null;
        private System.Net.Http.HttpRequestMessage _httpRequest = null;

        // _httpManager use : HttpClient, TraceException, LoadRetryTimeout, _InitLoadFromWeb
        private HttpManager_v3 _httpManager = null;
        //private UrlCache _urlCache = null;
        //private int _loadRetryTimeout = 0; // timeout in seconds, 0 = no timeout, -1 = endless timeout
        //private HttpClient _httpClient = null;

        // result
        //private bool _success = false;
        //private bool _loadFromWeb = false;
        //private bool _loadFromCache = false;
        //private UrlCachePathResult _urlCachePath = null;
        private DateTime _sendRequestTime;
        private TimeSpan _requestDuration;
        private System.Net.Http.HttpResponseMessage _httpResponse = null;

        //public bool Success { get { return _success; } }
        //public bool LoadFromWeb { get { return _loadFromWeb; } }
        //public bool LoadFromCache { get { return _loadFromCache; } }
        //public UrlCachePathResult UrlCachePath { get { return _urlCachePath; } }
        //public HttpResponseMessage HttpResponse { get { return _httpResponse; } }

        // HttpRequestParameters requestParameters = null
        public Http_v3(HttpRequest_v3 request, HttpManager_v3 httpManager)
        {
            _request = request;
            _httpManager = httpManager;
            //if (requestParameters != null)
            //    _requestParameters = requestParameters;
            //else
            //    _requestParameters = new HttpRequestParameters();
        }

        public virtual void Dispose()
        {
            if (_httpRequest != null)
            {
                _httpRequest.Dispose();
                _httpRequest = null;
            }
            if (_httpResponse != null)
            {
                _httpResponse.Dispose();
                _httpResponse = null;
            }
            if (_request != null)
            {
                _request.Dispose();
                _request = null;
            }
        }

        public async Task<HttpResult_v3<string>> LoadText()
        {
            //HttpResponseMessage response = await SendRequest();
            //return await response.Content.ReadAsStringAsync();
            bool success = false;
            string text = null;
            try
            {
                await SendRequest();
                text = await _httpResponse.Content.ReadAsStringAsync();
                _requestDuration = DateTime.Now - _sendRequestTime;
                success = true;
            }
            catch (Exception ex)
            {
                if (_httpManager.TraceException)
                    Trace.WriteLine($"{DateTime.Now:dd/MM/yyyy HH:mm:ss} error : loading \"{_request.Url}\" ({_request.Method}) {ex.Message}");
                else
                    throw;
            }
            return new HttpResult_v3<string>(success, GetHttpMessageResult, loadFromWeb: true) { Data = text };
        }

        // save http binary content response in a file (dont use character encoding, just save bytes)
        public async Task<HttpResult_v3> LoadToFile(string file)
        {
            bool success = false;
            try
            {
                await SendRequest();
                using (FileStream fs = zFile.Open(file, FileMode.Create, FileAccess.Write, FileShare.Read))
                    await _httpResponse.Content.CopyToAsync(fs);
                _requestDuration = DateTime.Now - _sendRequestTime;
                success = true;
            }
            catch (Exception ex)
            {
                if (_httpManager.TraceException)
                    Trace.WriteLine($"{DateTime.Now:dd/MM/yyyy HH:mm:ss} error : loading \"{_request.Url}\" ({_request.Method}) {ex.Message}");
                else
                    throw;
            }
            return new HttpResult_v3(success, GetHttpMessageResult, loadFromWeb: true);
        }

        //public async Task SaveRequest(string file)
        //{
        //}

        public HttpMessageResult GetHttpMessageResult()
        {
            if (_httpResponse == null)
                throw new PBException("unable to create HttpMessageResult, httpResponse is not defined");
            return new HttpMessageResult(_httpResponse, _sendRequestTime, _requestDuration, _request.Content?.ToString());
        }

        //private HttpResult<TData> Load<TData>(HttpRequest httpRequest, Func<Http_v2, TData> loadData, string subDirectory = null)
        //private async Task Load()
        //{
        //    //bool success = false;
        //    //bool loadFromWeb = false;
        //    //bool loadFromCache = false;
        //    //bool trace = false;
        //    //Http_v3 http = null;
        //    //TData data = default(TData);
        //    //try
        //    //{
        //    //if (_httpManager.UrlCache != null)
        //    //{
        //    //    //UrlCachePathResult urlCachePath
        //    //    _urlCachePath = _httpManager.UrlCache.GetUrlPathResult(_httpRequest, _httpRequest.CacheSubDirectory);
        //    //    string urlPath = _urlCachePath.Path;
        //    //    if (_httpRequest.ReloadFromWeb || !zFile.Exists(urlPath))
        //    //    {
        //    //        // cache file dont exists, create 2 cache files : file with content response, file with request response
        //    //        TraceLevel.WriteLine(1, $"Load from web \"{_httpRequest.Url}\" ({_httpRequest.Method})");
        //    //        //Http_v3 http2 = CreateHttp(httpRequest);
        //    //        //http2.LoadToFile(urlPath);
        //    //        //http2.SaveRequest(zpath.PathSetExtension(urlPath, ".request.json"));
        //    //        LoadToFile(urlPath);
        //    //        SaveRequest(zpath.PathSetExtension(urlPath, ".request.json"));
        //    //        _loadFromWeb = true;
        //    //        //trace = true;
        //    //    }
        //    //    else
        //    //    {
        //    //        // cache file exists, load request from cache file
        //    //        _loadFromCache = true;
        //    //        TraceLevel.WriteLine(1, "Load from cache \"{0}\"", _urlCachePath.Path);
        //    //        //http = CreateHttp(httpRequest);
        //    //    }
        //    //    //httpRequest.UrlCachePath = urlCachePath;
        //    //}
        //    //else
        //    //{
        //        //_loadFromWeb = true;
        //        TraceLevel.WriteLine(1, "Load from web \"{0}\" ({1})", _httpRequest.Url, _httpRequest.Method);
        //        _httpManager._InitLoadFromWeb();
        //        //http = CreateHttp(httpRequest);
        //        await SendRequest();
        //    //}

        //        //data = loadData(http);
        //        //_success = true;
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    if (_httpManager.TraceException)
        //    //        Trace.WriteLine($"{DateTime.Now:dd/MM/yyyy HH:mm:ss} error : loading \"{(_urlCachePath != null ? _urlCachePath.Path : _httpRequest.Url)}\" ({_httpRequest.Method}) {ex.Message}");
        //    //    else
        //    //        throw;
        //    //}
        //    //return new HttpResult<TData> { Success = success, LoadFromWeb = loadFromWeb, LoadFromCache = loadFromCache, Http = http, Data = data };
        //}

        private void CreateHttpRequestMessage()
        {
            _httpRequest = new System.Net.Http.HttpRequestMessage(HttpTools.GetHttpMethod(_request.Method), _request.Url);
            if (_request.Content != null)
                _httpRequest.Content = _request.Content.GetHttpContent();
        }

        private async Task SendRequest()
        {
            TraceLevel.WriteLine(1, "Load from web \"{0}\" ({1})", _request.Url, _request.Method);
            _httpManager.InitLoadFromWeb();
            CreateHttpRequestMessage();
            DateTime start = DateTime.Now;
            while (true)
            {
                try
                {
                    //Trace.WriteLine($"Http_v3.SendRequest() : send request");
                    _sendRequestTime = DateTime.Now;
                    _httpResponse = await _httpManager.HttpClient.SendAsync(_httpRequest);
                    break;
                }
                catch (Exception ex)
                {
                    //Trace.WriteLine($"Http_v3.SendRequest() : error \"{ex.Message}\"");
                    if (!Retry(ex))
                        throw;
                    else if (_httpManager.LoadRetryTimeout != -1)
                    {
                        start = DateTime.Now;
                        TimeSpan ts = DateTime.Now.Subtract(start);
                        if (ts.Seconds > _httpManager.LoadRetryTimeout)
                            throw;
                    }
                    Trace.WriteLine($"error : loading \"{_httpRequest.RequestUri}\" ({_httpRequest.Method}) {ex.Message}");
                }
            }
        }

        private bool Retry(Exception ex)
        {
            return false;
        }
    }
}
