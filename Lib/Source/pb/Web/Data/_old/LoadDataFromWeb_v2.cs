using System;
using System.IO;
using System.Xml.Linq;
using pb.Web.old;

namespace pb.Web.v1
{
    public class RequestFromWeb_v2
    {
        private string _url = null;
        private HttpRequestParameters_v1 _requestParameters = null;
        private bool _reloadFromWeb = false;
        private bool _loadImage = false;
        private bool _loadResult = false;
        private DateTime? _loadDate = null;

        public RequestFromWeb_v2(string url, HttpRequestParameters_v1 requestParameters = null, bool reload = false, bool loadImage = false)
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
        public bool LoadResult { get { return _loadResult; } set { _loadResult = value; } }
        public DateTime? LoadDate { get { return _loadDate; } set { _loadDate = value; } }

        public XDocument GetXmlDocument()
        {
            if (_loadResult == true)
                //return Http2.HtmlReader.XDocument;
                return Http_v3.Http.zGetXDocument();
            else
                return null;
        }
    }

    public delegate T GetDataFromWebDelegate_v2<T>(RequestFromWeb_v2 request);

    public class LoadDataFromWeb_v2<T>
    {
        private GetDataFromWebDelegate_v2<T> _getDataFromWeb = null;
        private UrlCache_v1 _urlCache = null;

        public LoadDataFromWeb_v2(GetDataFromWebDelegate_v2<T> getDataFromWeb, UrlCache_v1 urlCache = null)
        {
            _getDataFromWeb = getDataFromWeb;
            _urlCache = urlCache;
        }

        public void Dispose()
        {
        }

        protected void SetUrlCache(UrlCache_v1 urlCache)
        {
            _urlCache = urlCache;
        }

        public T Load(RequestFromWeb_v2 request)
        {
            string url = request.Url;

            if (_urlCache != null)
            {
                string urlPath = _urlCache.GetUrlPath(request.Url, request.RequestParameters);
                if (request.ReloadFromWeb || !File.Exists(urlPath))
                {
                    //if (!Http2.LoadToFile(request.Url, urlPath, request.RequestParameters))
                    if (!Http_v3.LoadToFile(request.Url, urlPath, request.RequestParameters))
                        return default(T);
                }
                url = urlPath;
            }
            //if (!Http2.LoadUrl(url, request.RequestParameters))
            if (!Http_v3.LoadUrl(url, request.RequestParameters))
                return default(T);
            request.LoadResult = true;
            return _getDataFromWeb(request);
        }

        //public XDocument GetXmlDocument()
        //{
        //    return Http2.HtmlReader.XDocument;
        //}
    }
}
