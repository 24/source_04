using System;
using System.IO;
using System.Xml.Linq;
using pb.IO;
using pb.Web.old;

namespace pb.Web.Data.old
{
    // not used : ???
    public class LoadFromWeb_v2
    {
        private UrlCache_v1 _urlCache = null;

        public LoadFromWeb_v2(UrlCache_v1 urlCache = null)
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

        public void Load(pb.Web.v1.RequestFromWeb_v2 request)
        {
            string url = request.Url;

            if (_urlCache != null)
            {
                string urlPath = _urlCache.GetUrlPath(request.Url, request.RequestParameters);
                if (request.ReloadFromWeb || !zFile.Exists(urlPath))
                {
                    //if (!Http2.LoadToFile(request.Url, urlPath, request.RequestParameters))
                    //    return;
                    if (!Http_v3.LoadToFile(request.Url, urlPath, request.RequestParameters))
                        return;
                }
                url = urlPath;
            }
            //if (!Http2.LoadUrl(url, request.RequestParameters))
            //    return;
            if (!Http_v3.LoadUrl(url, request.RequestParameters))
                return;
            request.LoadResult = true;
        }

        public XDocument GetXmlDocument()
        {
            //return Http2.HtmlReader.XDocument;
            return Http_v3.Http.zGetXDocument();
        }
    }
}
