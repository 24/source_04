using System;
using System.IO;
using System.Xml.Linq;
using pb.Web.old;

namespace pb.Web
{
    public abstract class LoadDataFromWeb_v1<T>
    {
        private string _url = null;
        private HttpRequestParameters_v1 _requestParameters = null;
        private bool _reload = false;
        private UrlCache_v1 _urlCache = null;
        private bool _loadResult = false;
        private DateTime? _loadDate = null;
        private T _data;

        public LoadDataFromWeb_v1(string url, HttpRequestParameters_v1 requestParameters = null, bool reload = false)
        {
            _url = url;
            _requestParameters = requestParameters;
            _reload = reload;
        }

        public void Dispose()
        {
        }

        public string Url { get { return _url; } }
        public HttpRequestParameters_v1 RequestParameters { get { return _requestParameters; } }
        public bool LoadResult { get { return _loadResult; } }
        public DateTime? LoadDate { get { return _loadDate; } }
        public T Data { get { return _data; } }

        protected void SetRequestParameters(HttpRequestParameters_v1 requestParameters)
        {
            _requestParameters = requestParameters;
        }

        protected void SetUrlCache(UrlCache_v1 urlCache)
        {
            _urlCache = urlCache;
        }

        public bool Load()
        {
            _loadResult = false;
            string url = _url;

            if (_urlCache != null)
            {
                //string urlPath = _urlCache.UrlPath;
                string urlPath = _urlCache.GetUrlPath(url, _requestParameters);
                if (_reload || !File.Exists(urlPath))
                {
                    //if (!Http2.LoadToFile(_url, urlPath, _requestParameters))
                    if (!Http_v3.LoadToFile(_url, urlPath, _requestParameters))
                        return false;
                }
                url = urlPath;
            }
            //if (!Http2.LoadUrl(url, _requestParameters))
            if (!Http_v3.LoadUrl(url, _requestParameters))
                return false;
            _loadResult = true;
            _data = GetData();
            return true;
        }

        //protected
        public XDocument GetXmlDocument()
        {
            //return Http2.HtmlReader.XDocument;
            return Http_v3.Http.zGetXDocument();
        }

        protected abstract T GetData();
    }
}
