using pb.IO;
using System;

namespace pb.Web.Http
{
    public class HttpResult
    {
        public bool Success;
        public bool LoadFromWeb;
        public bool LoadFromCache;
        public Http_v2 Http;
        //public UrlCachePathResult UrlCachePath;
    }

    public class HttpResult<T> : HttpResult
    {
        public T Data;
    }

    public partial class HttpManager_v2
    {
        private bool _traceException = false;
        //private int _loadRepeatIfError = 1;
        private int _loadRetryTimeout = 10;                                // timeout in seconds, 0 = no timeout, -1 = endless timeout
        //private bool _exportResult = false;
        //private string _exportDirectory = null;
        private UrlCache _urlCache = null;
        private HttpRequestParameters _requestParameters = null;
        private Action _initLoadFromWeb = null;
        private bool _firstLoadFromWeb = true;
        //private Func<HttpRequestParameters> _getHttpRequestParameters = null;

        public bool TraceException { get { return _traceException; } set { _traceException = value; } }
        public int LoadRetryTimeout { get { return _loadRetryTimeout; } set { _loadRetryTimeout = value; } }
        public UrlCache UrlCache { get { return _urlCache; } set { _urlCache = value; } }
        public HttpRequestParameters RequestParameters { get { return _requestParameters; } set { _requestParameters = value; } }
        public Action InitLoadFromWeb { get { return _initLoadFromWeb; } set { _initLoadFromWeb = value; } }

        protected string GetCacheDirectory()
        {
            if (_urlCache != null)
                return _urlCache.CacheDirectory;
            else
                return null;
        }

        private HttpResult<TData> Load<TData>(HttpRequest httpRequest, Func<Http_v2, TData> loadData, string subDirectory = null)
        {
            bool success = false;
            bool loadFromWeb = false;
            bool loadFromCache = false;
            bool trace = false;
            Http_v2 http = null;
            TData data = default(TData);
            try
            {
                if (_urlCache != null)
                {
                    //HttpResult<UrlCachePathResult> cachePathResult = LoadHttpToCache(httpRequest);
                    //httpRequest.UrlCachePath = cachePathResult.Data;

                    UrlCachePathResult urlCachePath = _urlCache.GetUrlPathResult(httpRequest, subDirectory);
                    string urlPath = urlCachePath.Path;
                    if (httpRequest.ReloadFromWeb || !zFile.Exists(urlPath))
                    {
                        TraceLevel.WriteLine(1, "Load \"{0}\" ({1})", httpRequest.Url, httpRequest.Method);
                        Http_v2 http2 = CreateHttp(httpRequest);
                        http2.LoadToFile(urlPath, true);
                        loadFromWeb = true;
                        trace = true;
                    }
                    else
                        loadFromCache = true;
                    httpRequest.UrlCachePath = urlCachePath;
                }
                else
                    loadFromWeb = true;

                if (!trace)
                    TraceLevel.WriteLine(1, "Load \"{0}\" ({1})", httpRequest.UrlCachePath != null ? httpRequest.UrlCachePath.Path : httpRequest.Url, httpRequest.Method);

                http = CreateHttp(httpRequest);

                //text = http.LoadText();
                data = loadData(http);
                success = true;
            }
            catch (Exception ex)
            {
                if (_traceException)
                    Trace.WriteLine("{0:dd/MM/yyyy HH:mm:ss} Error : loading \"{1}\" ({2}) {3}", DateTime.Now, httpRequest.UrlCachePath != null ? httpRequest.UrlCachePath.Path : httpRequest.Url, httpRequest.Method, ex.Message);
                else
                    throw;
            }
            return new HttpResult<TData> { Success = success, LoadFromWeb = loadFromWeb, LoadFromCache = loadFromCache, Http = http, Data = data };
        }

        // HttpRequestParameters requestParameters = null
        //private HttpResult<UrlCachePathResult> LoadHttpToCache(HttpRequest httpRequest, string subDirectory = null)
        //{
        //    HttpResult<UrlCachePathResult> cachePathResult = new HttpResult<UrlCachePathResult>();
        //    //UrlCachePathResult urlCachePath = _urlCache.GetUrlPathResult(httpRequest, subDirectory);
        //    cachePathResult.Data = _urlCache.GetUrlPathResult(httpRequest, subDirectory);
        //    string urlPath = cachePathResult.Data.Path;
        //    if (httpRequest.ReloadFromWeb || !zFile.Exists(urlPath))
        //    {
        //        //_InitLoadFromWeb(httpRequest);
        //        //if (!HttpManager.CurrentHttpManager.LoadToFile(httpRequest, urlPath, _urlCache.SaveRequest, _GetHttpRequestParameters()))
        //        //    return webResult;
        //        Trace.WriteLine(1, "Load \"{0}\" ({1})", httpRequest.Url, httpRequest.Method);
        //        Http_v2 http = CreateHttp(httpRequest);
        //        http.LoadToFile(urlPath, true);
        //        cachePathResult.Http = http;
        //    }
        //    return cachePathResult;
        //}

        // HttpRequestParameters requestParameters = null, string exportFile = null, bool setExportFileExtension = false
        // bool cacheFile = false
        public Http_v2 CreateHttp(HttpRequest httpRequest)
        {
            //if (!cacheFile)
            if (httpRequest.UrlCachePath == null)
                _InitLoadFromWeb();
            Http_v2 http = new Http_v2(httpRequest, _requestParameters);
            //http.HttpRetry += new Http.fnHttpRetry(LoadRetryEvent);
            http.LoadRetryTimeout = _loadRetryTimeout;
            //if (exportFile != null)
            //{
            //    http.ExportFile = exportFile;
            //    http.SetExportFileExtension = setExportFileExtension;
            //}
            //else if (_exportResult && _exportDirectory != null)
            //{
            //    http.ExportFile = GetNewHttpFileName(httpRequest);
            //    http.SetExportFileExtension = true;
            //}
            return http;
        }

        //private string GetNewHttpFileName(HttpRequest httpRequest, string ext = null)
        //{
        //    return zfile.GetNewIndexedFileName(_exportDirectory) + "_" + zurl.UrlToFileName(httpRequest.Url, UrlFileNameType.Host | UrlFileNameType.Path | UrlFileNameType.Ext | UrlFileNameType.Query, ext);
        //}

        private void _InitLoadFromWeb()
        {
            // httpRequest.Url.StartsWith("http://")
            if (_firstLoadFromWeb)
            {
                if (_initLoadFromWeb != null)
                    _initLoadFromWeb();
                _firstLoadFromWeb = false;
            }
        }
    }
}
