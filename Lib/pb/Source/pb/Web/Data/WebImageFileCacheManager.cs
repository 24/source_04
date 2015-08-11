using System;
using System.Drawing;
using System.IO;
using pb.Data;
using pb.IO;

namespace pb.Web
{
    public class WebImageFileCacheManager : WebImageCacheManager
    {
        protected UrlCache _urlCache;

        public WebImageFileCacheManager(string directory)
        {
            //_urlCache = new UrlCache(directory, UrlFileNameType.Path | UrlFileNameType.Host, (url, requestParameters) => zurl.GetDomain(url));
            _urlCache = new UrlCache(directory);
            _urlCache.UrlFileNameType = UrlFileNameType.Path | UrlFileNameType.Host;
            _urlCache.GetUrlSubDirectoryFunction = httpRequest => zurl.GetDomain(httpRequest.Url);
        }

        //public override Image LoadImage(string url, HttpRequestParameters requestParameters = null)
        public override Image LoadImage(string url, HttpRequestParameters requestParameters = null)
        {
            //string file = _urlCache.GetUrlSubPath(url, requestParameters);
            HttpRequest httpRequest = new HttpRequest { Url = url };
            string file = _urlCache.GetUrlSubPath(httpRequest);
            string urlPath = zPath.Combine(_urlCache.CacheDirectory, file);
            if (!zFile.Exists(urlPath))
            {
                //if (!Http2.LoadToFile(url, urlPath, requestParameters))
                if (!HttpManager.CurrentHttpManager.LoadToFile(httpRequest, urlPath, requestParameters))
                {
                    urlPath = null;
                    file = null;
                }
            }
            if (urlPath != null)
                return zimg.LoadFromFile(urlPath);
            return null;
        }
    }
}
