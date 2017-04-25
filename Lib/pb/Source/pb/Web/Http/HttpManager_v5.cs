using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

// How do I set a cookie on HttpClient's HttpRequestMessage http://stackoverflow.com/questions/12373738/how-do-i-set-a-cookie-on-httpclients-httprequestmessage
// todo :
//   - ok manage cookies
//   - ok manage user agent ... (HttpRequestParameters)

namespace pb.Web.Http
{
    public class HttpManager_v5 : IDisposable
    {
        private static string _defaultUserAgent = "pib/0.1";
        //private static string _defaultAccept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";

        private HttpClient _httpClient = null;
        private HttpClientHandler _httpClientHandler = null;

        private Encoding _defaultEncoding = Encoding.UTF8;
        private bool _traceException = false;
        private int _loadRetryTimeout = 10;                                // timeout in seconds, 0 = no timeout, -1 = endless timeout
        //private UrlCache _urlCache = null;
        private HttpCacheManager _httpCacheManager = null;
        //private HttpRequestParameters _requestParameters = null;
        private Action _initLoadFromWeb = null;
        private bool _firstLoadFromWeb = true;

        public HttpClient HttpClient { get { return _httpClient; } }
        public HttpClientHandler HttpClientHandler { get { return _httpClientHandler; } }
        public Encoding DefaultEncoding { get { return _defaultEncoding; } set { _defaultEncoding = value; } }
        public bool TraceException { get { return _traceException; } set { _traceException = value; } }
        public int LoadRetryTimeout { get { return _loadRetryTimeout; } set { _loadRetryTimeout = value; } }
        //public UrlCache UrlCache { get { return _urlCache; } set { _urlCache = value; } }
        //public HttpCache HttpCache { get { return _httpCache; } set { _httpCache = value; } }
        //public HttpRequestParameters RequestParameters { get { return _requestParameters; } set { _requestParameters = value; } }
        //public Action InitLoadFromWeb { get { return _initLoadFromWeb; } set { _initLoadFromWeb = value; } }

        public HttpManager_v5()
        {
            _httpClientHandler = new HttpClientHandler();
            _httpClient = new HttpClient(_httpClientHandler);
            //SetDefaultHeaders();
        }

        public void Dispose()
        {
            if (_httpClient != null)
            {
                _httpClient.Dispose();
                _httpClient = null;
            }
            if (_httpClientHandler != null)
            {
                _httpClientHandler.Dispose();
                _httpClientHandler = null;
            }
        }

        public void SetCacheManager(UrlCache urlCache)
        {
            _httpCacheManager = new HttpCacheManager(this, urlCache);
        }

        public void ActiveCookies()
        {
            _httpClientHandler.CookieContainer = new CookieContainer();
        }

        public void SetDefaultHeaders()
        {
            _httpClient.DefaultRequestHeaders.UserAgent.zSet(_defaultUserAgent);
            //_httpClient.DefaultRequestHeaders.Accept.zSet(_defaultAccept);
        }

        //public HttpResult<string> LoadText(HttpRequest_v3 httpRequest)
        public async Task<HttpResult_v5<string>> LoadText(HttpRequest_v5 httpRequest)
        {
            if (_httpCacheManager != null)
                return await _httpCacheManager.LoadText(httpRequest);
            else
                return await new Http_v5(httpRequest, this).LoadText();
            //return Load(httpRequest, http => http.LoadText());
            //bool success = false;
            //try
            //{

            //}
            //catch (Exception ex)
            //{
            //    if (_traceException)
            //        Trace.WriteLine($"{DateTime.Now:dd/MM/yyyy HH:mm:ss} Error : loading \"{(httpRequest.UrlCachePath != null ? httpRequest.UrlCachePath.Path : httpRequest.Url)}\" ({httpRequest.Method}) {ex.Message}");
            //    else
            //        throw;
            //}
            //return new HttpResult<string> { Success = success, LoadFromWeb = loadFromWeb, LoadFromCache = loadFromCache, Http = http, Data = data };
        }

        public async Task<HttpResult_v5> LoadToFile(HttpRequest_v5 httpRequest, string file)
        {
            return await new Http_v5(httpRequest, this).LoadToFile(file);
        }

        //public Http_v2 CreateHttp(HttpRequest httpRequest)
        //public Http_v3 CreateHttp(HttpRequest_v3 httpRequest)
        //{
        //    //if (httpRequest.UrlCachePath == null)
        //    //    _InitLoadFromWeb();
        //    //Http_v2 http = new Http_v2(httpRequest, _requestParameters);
        //    Http_v3 http = new Http_v3(httpRequest, this);
        //    //http.LoadRetryTimeout = _loadRetryTimeout;
        //    return http;
        //}

        public void SetInitLoadFromWeb(Action initLoadFromWeb)
        {
            _initLoadFromWeb = initLoadFromWeb;
        }

        public void InitLoadFromWeb()
        {
            if (_firstLoadFromWeb)
            {
                _initLoadFromWeb?.Invoke();
                _firstLoadFromWeb = false;
            }
        }

        //protected string GetCacheDirectory()
        //{
        //    if (_urlCache != null)
        //        return _urlCache.CacheDirectory;
        //    else
        //        return null;
        //}
    }
}
