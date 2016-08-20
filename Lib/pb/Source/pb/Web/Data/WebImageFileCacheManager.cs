using System.Drawing;
using pb.Data;
using pb.IO;

namespace pb.Web
{
    public class WebImageFileCacheManager : WebImageCacheManager
    {
        protected UrlCache _urlCache;
        //protected bool _exportRequest = false;

        //public WebImageFileCacheManager(string directory)
        //{
        //    _urlCache = new UrlCache(directory);
        //    _urlCache.UrlFileNameType = UrlFileNameType.Path | UrlFileNameType.Host;
        //    _urlCache.GetUrlSubDirectoryFunction = httpRequest => zurl.GetDomain(httpRequest.Url);
        //}

        public WebImageFileCacheManager(UrlCache urlCache)
        {
            _urlCache = urlCache;
        }

        public override Image LoadImage(string url, HttpRequestParameters requestParameters = null, bool refreshImage = false)
        {
            HttpRequest httpRequest = new HttpRequest { Url = url };
            string file = _urlCache.GetUrlSubPath(httpRequest);
            string urlPath = zPath.Combine(_urlCache.CacheDirectory, file);
            if (refreshImage || !zFile.Exists(urlPath))
            {
                if (!HttpManager.CurrentHttpManager.LoadToFile(httpRequest, urlPath, _urlCache.SaveRequest, requestParameters))
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
