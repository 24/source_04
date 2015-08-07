using System;
using System.IO;
using pb.IO;

namespace pb.Web
{
    public class LoadFromWebManager_v4
    {
        protected UrlCache _urlCache = null;
        protected bool _firstLoadFromWeb = true;

        public void Dispose()
        {
        }

        protected virtual void InitLoadFromWeb()
        {
        }

        protected virtual HttpRequestParameters GetHttpRequestParameters()
        {
            return new HttpRequestParameters();
        }

        public LoadDataFromWeb_v4 Load(RequestFromWeb_v4 webRequest)
        {
            LoadDataFromWeb_v4 loadDataFromWeb = new LoadDataFromWeb_v4 { WebRequest = webRequest };

            DateTime loadFromWebDate;

            HttpRequest httpRequest = webRequest.HttpRequest;

            if (_urlCache != null)
            {
                string urlPath = _urlCache.GetUrlPath(httpRequest);
                if (webRequest.ReloadFromWeb || !zFile.Exists(urlPath))
                {
                    if (_firstLoadFromWeb && httpRequest.Url.StartsWith("http://"))
                    {
                        InitLoadFromWeb();
                        _firstLoadFromWeb = false;
                    }
                    //if (!HttpManager.CurrentHttpManager.LoadToFile(httpRequest, urlPath, GetHttpRequestParameters()))
                    //    return default(T);
                    if (!HttpManager.CurrentHttpManager.LoadToFile(httpRequest, urlPath, GetHttpRequestParameters()))
                        return loadDataFromWeb;
                }
                httpRequest = new HttpRequest { Url = urlPath };
                // get last write time as loadFromWebDate, dont take creation time because creation time is modified when copying the file
                //loadFromWebDate = new FileInfo(urlPath).LastWriteTime;
                loadFromWebDate = zFile.CreateFileInfo(urlPath).LastWriteTime;
            }
            else
                loadFromWebDate = DateTime.Now;
            if (_firstLoadFromWeb && httpRequest.Url.StartsWith("http://"))
            {
                InitLoadFromWeb();
                _firstLoadFromWeb = false;
            }
            loadDataFromWeb.Http = HttpManager.CurrentHttpManager.Load(httpRequest, GetHttpRequestParameters());
            //if (loadDataFromWeb.Http == null)
            //    return default(T);
            if (loadDataFromWeb.Http != null)
            {
                loadDataFromWeb.LoadResult = true;
                loadDataFromWeb.LoadFromWebDate = loadFromWebDate;
            }
            return loadDataFromWeb;
        }
    }

    public class LoadFromWebManager_v5
    {
        private UrlCache _urlCache = null;
        private Action _initLoadFromWeb = null;
        private Func<HttpRequestParameters> _getHttpRequestParameters = null;
        private bool _firstLoadFromWeb = true;

        public void Dispose()
        {
        }

        public UrlCache UrlCache { get { return _urlCache; } set { _urlCache = value; } }
        public Action InitLoadFromWeb { get { return _initLoadFromWeb; } set { _initLoadFromWeb = value; } }
        public Func<HttpRequestParameters> GetHttpRequestParameters { get { return _getHttpRequestParameters; } set { _getHttpRequestParameters = value; } }

        //protected virtual void InitLoadFromWeb()
        //{
        //}

        //protected virtual HttpRequestParameters_new GetHttpRequestParameters()
        //{
        //    return new HttpRequestParameters_new();
        //}

        public LoadDataFromWeb_v4 Load(RequestFromWeb_v4 webRequest)
        {
            LoadDataFromWeb_v4 loadDataFromWeb = new LoadDataFromWeb_v4 { WebRequest = webRequest };

            DateTime loadFromWebDate;

            HttpRequest httpRequest = webRequest.HttpRequest;

            if (_urlCache != null)
            {
                string urlPath = _urlCache.GetUrlPath(httpRequest);
                if (webRequest.ReloadFromWeb || !zFile.Exists(urlPath))
                {
                    _InitLoadFromWeb(httpRequest);
                    if (!HttpManager.CurrentHttpManager.LoadToFile(httpRequest, urlPath, _GetHttpRequestParameters()))
                        return loadDataFromWeb;
                }
                httpRequest = new HttpRequest { Url = urlPath };
                // get last write time as loadFromWebDate, dont take creation time because creation time is modified when copying the file
                //loadFromWebDate = new FileInfo(urlPath).LastWriteTime;
                loadFromWebDate = zFile.CreateFileInfo(urlPath).LastWriteTime;
            }
            else
                loadFromWebDate = DateTime.Now;
            _InitLoadFromWeb(httpRequest);
            loadDataFromWeb.Http = HttpManager.CurrentHttpManager.Load(httpRequest, _GetHttpRequestParameters());
            if (loadDataFromWeb.Http != null)
            {
                loadDataFromWeb.LoadResult = true;
                loadDataFromWeb.LoadFromWebDate = loadFromWebDate;
            }
            return loadDataFromWeb;
        }

        private void _InitLoadFromWeb(HttpRequest httpRequest)
        {
            if (_firstLoadFromWeb && httpRequest.Url.StartsWith("http://"))
            {
                //InitLoadFromWeb();
                if (_initLoadFromWeb != null)
                    _initLoadFromWeb();
                _firstLoadFromWeb = false;
            }
        }

        private HttpRequestParameters _GetHttpRequestParameters()
        {
            if (_getHttpRequestParameters != null)
                return _getHttpRequestParameters();
            else
                return new HttpRequestParameters();
        }
    }
}
