using System;
using System.Xml.Linq;
using pb.Data.Xml;
using pb.IO;

// to do
//  gérer loadFromWebDate du cache

namespace pb.Web
{
    public class UrlCachePathResult
    {
        public string Path;
        public string SubPath;
    }

    public class UrlCache
    {
        protected string _cacheDirectory = null;
        protected UrlFileNameType _urlFileNameType = UrlFileNameType.Path;
        protected bool _saveRequest = false;
        protected Func<HttpRequest, string> _getUrlSubDirectory = null;

        public UrlCache(string cacheDirectory)
        {
            _cacheDirectory = cacheDirectory;
        }

        public string CacheDirectory { get { return _cacheDirectory; } }
        public UrlFileNameType UrlFileNameType { get { return _urlFileNameType; } }  // set { _urlFileNameType = value; }
        public bool SaveRequest { get { return _saveRequest; } }
        public Func<HttpRequest, string> GetUrlSubDirectory { get { return _getUrlSubDirectory; } set { _getUrlSubDirectory = value; } }

        public string GetUrlPath(HttpRequest httpRequest)
        {
            return zPath.Combine(_cacheDirectory, GetUrlSubPath(httpRequest));
        }

        public UrlCachePathResult GetUrlPathResult(HttpRequest httpRequest, string subDirectory = null)
        {
            string subPath = GetUrlSubPath(httpRequest);
            if (subDirectory != null)
                subPath = zPath.Combine(subDirectory, subPath);
            return new UrlCachePathResult { Path = zPath.Combine(_cacheDirectory, subPath), SubPath = subPath };
        }

        public string GetUrlSubPath(HttpRequest httpRequest)
        {
            string file = GetUrlFilename(httpRequest);
            string dir = _GetUrlSubDirectory(httpRequest);
            if (dir != null)
                file = zPath.Combine(dir, file);
            return file;
        }

        private string _GetUrlSubDirectory(HttpRequest httpRequest)
        {
            if (_getUrlSubDirectory != null)
                return _getUrlSubDirectory(httpRequest);
            else
                return null;
        }

        private string GetUrlFilename(HttpRequest httpRequest)
        {
            return zurl.UrlToFileName(httpRequest, _urlFileNameType);
        }

        public static UrlCache Create(XElement xe)
        {
            if (xe != null && xe.zXPathValue("UseUrlCache").zTryParseAs(false))
            {
                UrlCache urlCache = new UrlCache(xe.zXPathExplicitValue("CacheDirectory"));
                urlCache._urlFileNameType = zurl.GetUrlFileNameType(xe.zXPathValue("CacheUrlFileNameType", "Path"));
                urlCache._saveRequest = xe.zXPathValue("CacheSaveRequest").zTryParseAs(false);
                return urlCache;
            }
            else
                return null;
        }
    }
}
