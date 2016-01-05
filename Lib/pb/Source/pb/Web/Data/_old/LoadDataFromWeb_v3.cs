using System;
using System.Xml.Linq;
using pb.IO;
using pb.Web.old;

namespace pb.Web.old
{
    public class RequestFromWeb_v3
    {
        private string _url = null;
        private HttpRequestParameters_v1 _requestParameters = null;
        private bool _reloadFromWeb = false;
        private bool _loadImage = false;

        public RequestFromWeb_v3(string url, HttpRequestParameters_v1 requestParameters = null, bool reload = false, bool loadImage = false)
        {
            _url = url;
            _requestParameters = requestParameters;
            _reloadFromWeb = reload;
            _loadImage = loadImage;
        }

        public string Url { get { return _url; } }
        public HttpRequestParameters_v1 RequestParameters { get { return _requestParameters; } }
        public bool ReloadFromWeb { get { return _reloadFromWeb; } }
        public bool LoadImage { get { return _loadImage; } }
    }

    public class LoadDataFromWeb_v3
    {
        public RequestFromWeb_v3 request;
        public bool loadResult = false;
        //public DateTime? loadDate = null;
        public DateTime loadFromWebDate;

        public XDocument GetXmlDocument()
        {
            if (loadResult == true)
                //return Http2.HtmlReader.XDocument;
                return Http_v3.Http.zGetXDocument();
            else
                return null;
        }
    }

    public abstract class LoadDataFromWebManager_v3<T>
    {
        protected UrlCache_v1 _urlCache = null;
        protected bool _firstLoadFromWeb = true;

        public LoadDataFromWebManager_v3(UrlCache_v1 urlCache = null)
        {
            _urlCache = urlCache;
        }

        public void Dispose()
        {
        }

        protected void SetUrlCache(UrlCache_v1 urlCache)
        {
            _urlCache = urlCache;
        }

        protected virtual void InitLoadFromWeb()
        {
        }

        public T Load(RequestFromWeb_v3 request)
        {
            LoadDataFromWeb_v3 loadDataFromWeb = new LoadDataFromWeb_v3 { request = request };

            DateTime loadFromWebDate;

            string url = request.Url;

            if (_urlCache != null)
            {
                string urlPath = _urlCache.GetUrlPath(url, request.RequestParameters);
                if (request.ReloadFromWeb || !zFile.Exists(urlPath))
                {
                    if (_firstLoadFromWeb && url.StartsWith("http://"))
                    {
                        InitLoadFromWeb();
                        _firstLoadFromWeb = false;
                    }
                    //if (!Http2.LoadToFile(url, urlPath, request.RequestParameters))
                    //    return default(T);
                    if (!Http_v3.LoadToFile(url, urlPath, request.RequestParameters))
                        return default(T);
                }
                url = urlPath;
                // get last write time as loadFromWebDate, dont take creation time because creation time is modified when copying the file
                //loadFromWebDate = new FileInfo(urlPath).LastWriteTime;
                loadFromWebDate = zFile.CreateFileInfo(urlPath).LastWriteTime;
            }
            else
                loadFromWebDate = DateTime.Now;
            if (_firstLoadFromWeb && url.StartsWith("http://"))
            {
                InitLoadFromWeb();
                _firstLoadFromWeb = false;
            }
            //if (!Http2.LoadUrl(url, request.RequestParameters))
            //    return default(T);
            if (!Http_v3.LoadUrl(url, request.RequestParameters))
                return default(T);
            loadDataFromWeb.loadResult = true;
            loadDataFromWeb.loadFromWebDate = loadFromWebDate;
            return GetDataFromWeb(loadDataFromWeb);
        }

        protected abstract T GetDataFromWeb(LoadDataFromWeb_v3 loadDataFromWeb);
    }

    public class RequestFromWeb_v4
    {
        private HttpRequest _httpRequest = null;
        private bool _reloadFromWeb = false;
        private bool _loadImage = false;

        public RequestFromWeb_v4(HttpRequest request, bool reload = false, bool loadImage = false)
        {
            _httpRequest = request;
            _reloadFromWeb = reload;
            _loadImage = loadImage;
        }

        public HttpRequest HttpRequest { get { return _httpRequest; } }
        public bool ReloadFromWeb { get { return _reloadFromWeb; } }
        public bool LoadImage { get { return _loadImage; } }
    }

    public class LoadDataFromWeb_v4
    {
        public RequestFromWeb_v4 WebRequest;
        public Http Http;
        public bool LoadResult = false;
        public DateTime LoadFromWebDate;

        //public XDocument GetXmlDocument()
        //{
        //    if (!LoadResult)
        //        throw new PBException("Error no data result");
        //    //return Http2.HtmlReader.XDocument;
        //    return Http.zGetXmlDocument();
        //}
    }

    public abstract class LoadDataFromWebManager_v4<T> : LoadFromWebManager_v4
    {
        //protected UrlCache_new _urlCache = null;
        //protected bool _firstLoadFromWeb = true;

        //public void Dispose()
        //{
        //}

        //protected virtual void InitLoadFromWeb()
        //{
        //}

        //protected virtual HttpRequestParameters_new GetHttpRequestParameters()
        //{
        //    return new HttpRequestParameters_new();
        //}

        //public T LoadData(RequestFromWeb_new webRequest)
        //{
        //    LoadDataFromWeb_new loadDataFromWeb = new LoadDataFromWeb_new { WebRequest = webRequest };

        //    DateTime loadFromWebDate;

        //    HttpRequest httpRequest = webRequest.HttpRequest;

        //    if (_urlCache != null)
        //    {
        //        string urlPath = _urlCache.GetUrlPath(httpRequest);
        //        if (webRequest.ReloadFromWeb || !File.Exists(urlPath))
        //        {
        //            if (_firstLoadFromWeb && httpRequest.Url.StartsWith("http://"))
        //            {
        //                InitLoadFromWeb();
        //                _firstLoadFromWeb = false;
        //            }
        //            if (!HttpManager.CurrentHttpManager.LoadToFile(httpRequest, urlPath, GetHttpRequestParameters()))
        //                return default(T);
        //        }
        //        httpRequest = new HttpRequest { Url = urlPath };
        //        // get last write time as loadFromWebDate, dont take creation time because creation time is modified when copying the file
        //        loadFromWebDate = new FileInfo(urlPath).LastWriteTime;
        //    }
        //    else
        //        loadFromWebDate = DateTime.Now;
        //    if (_firstLoadFromWeb && httpRequest.Url.StartsWith("http://"))
        //    {
        //        InitLoadFromWeb();
        //        _firstLoadFromWeb = false;
        //    }
        //    loadDataFromWeb.Http = HttpManager.CurrentHttpManager.Load(httpRequest, GetHttpRequestParameters());
        //    if (loadDataFromWeb.Http == null)
        //        return default(T);
        //    loadDataFromWeb.LoadResult = true;
        //    loadDataFromWeb.LoadFromWebDate = loadFromWebDate;
        //    return GetData(loadDataFromWeb);
        //}

        public T LoadData(RequestFromWeb_v4 webRequest)
        {
            LoadDataFromWeb_v4 loadDataFromWeb = Load(webRequest);
            if (loadDataFromWeb.LoadResult)
                return GetData(loadDataFromWeb);
            else
                return default(T);
        }

        protected abstract T GetData(LoadDataFromWeb_v4 loadDataFromWeb);
    }

    public class LoadDataFromWebManager_v5<T> : LoadFromWebManager_v5
    {
        protected Func<LoadDataFromWeb_v4, T> _getData = null;

        //public LoadDataFromWebManager_new2(Func<LoadDataFromWeb_new, T> getData)
        //{
        //    _getData = getData;
        //}

        public Func<LoadDataFromWeb_v4, T> GetData { get { return _getData; } set { _getData = value; } }

        public T LoadData(RequestFromWeb_v4 webRequest)
        {
            LoadDataFromWeb_v4 loadDataFromWeb = Load(webRequest);
            if (loadDataFromWeb.LoadResult)
                return _getData(loadDataFromWeb);
            else
                return default(T);
        }
    }
}
