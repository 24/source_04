﻿using System;
using pb.IO;

namespace pb.Web.Data
{
    public class WebRequest
    {
        private HttpRequest _httpRequest = null;
        private bool _reloadFromWeb = false;
        private bool _refreshDocumentStore = false;
        private WebImageRequest _imageRequest = null;

        public HttpRequest HttpRequest { get { return _httpRequest; } set { _httpRequest = value; } }
        public bool ReloadFromWeb { get { return _reloadFromWeb; } set { _reloadFromWeb = value; } }
        public WebImageRequest ImageRequest { get { return _imageRequest; } set { _imageRequest = value; } }
        public bool RefreshDocumentStore { get { return _refreshDocumentStore; } set { _refreshDocumentStore = value; } }
    }

    public class WebImageRequest
    {
        public bool LoadImageFromWeb;
        public bool LoadMissingImageFromWeb;
        public bool LoadImageToData;
        public bool RefreshImage;
    }

    public class WebResult
    {
        public WebRequest WebRequest;
        public Http Http;
        public bool LoadResult = false;
        public DateTime LoadFromWebDate;
        public UrlCachePathResult UrlCachePathResult;
    }

    public class WebLoadManager
    {
        private UrlCache _urlCache = null;
        //private bool _exportRequest = false;
        private Action _initLoadFromWeb = null;
        private Func<HttpRequestParameters> _getHttpRequestParameters = null;
        private bool _firstLoadFromWeb = true;

        public void Dispose()
        {
        }

        public UrlCache UrlCache { get { return _urlCache; } set { _urlCache = value; } }
        //public bool ExportRequest { get { return _exportRequest; } set { _exportRequest = value; } }
        public Action InitLoadFromWeb { get { return _initLoadFromWeb; } set { _initLoadFromWeb = value; } }
        public Func<HttpRequestParameters> GetHttpRequestParameters { get { return _getHttpRequestParameters; } set { _getHttpRequestParameters = value; } }

        public WebResult Load(WebRequest webRequest)
        {
            WebResult webResult = new WebResult { WebRequest = webRequest };

            DateTime loadFromWebDate;

            HttpRequest httpRequest = webRequest.HttpRequest;

            if (_urlCache != null)
            {
                //string urlPath = _urlCache.GetUrlPath(httpRequest);
                webResult.UrlCachePathResult = _urlCache.GetUrlPathResult(httpRequest);
                string urlPath = webResult.UrlCachePathResult.Path;
                if (webRequest.ReloadFromWeb || !zFile.Exists(urlPath))
                {
                    _InitLoadFromWeb(httpRequest);
                    if (!HttpManager.CurrentHttpManager.LoadToFile(httpRequest, urlPath, _urlCache.SaveRequest, _GetHttpRequestParameters()))
                        return webResult;
                }
                httpRequest = new HttpRequest { Url = urlPath };
                // get last write time as loadFromWebDate, dont take creation time because creation time is modified when copying the file
                //loadFromWebDate = new FileInfo(urlPath).LastWriteTime;
                loadFromWebDate = zFile.CreateFileInfo(urlPath).LastWriteTime;
            }
            else
                loadFromWebDate = DateTime.Now;
            _InitLoadFromWeb(httpRequest);
            webResult.Http = HttpManager.CurrentHttpManager.Load(httpRequest, _GetHttpRequestParameters());
            if (webResult.Http != null)
            {
                webResult.LoadResult = true;
                webResult.LoadFromWebDate = loadFromWebDate;
            }
            return webResult;
        }

        private void _InitLoadFromWeb(HttpRequest httpRequest)
        {
            if (_firstLoadFromWeb && httpRequest.Url.StartsWith("http://"))
            {
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
