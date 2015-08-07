using System;
using System.IO;
using System.Xml.Linq;
using pb.Data.Xml;
using pb.IO;
using pb.Web.old;

// to do
//  gérer loadFromWebDate du cache

namespace pb.Web
{
    public class UrlCache
    {
        protected string _cacheDirectory = null;
        protected UrlFileNameType _urlFileNameType = UrlFileNameType.Path;
        protected Func<HttpRequest, string> _getUrlSubDirectory = null;

        public UrlCache(string cacheDirectory)
        {
            _cacheDirectory = cacheDirectory;
        }

        public string CacheDirectory { get { return _cacheDirectory; } }
        public UrlFileNameType UrlFileNameType { get { return _urlFileNameType; } set { _urlFileNameType = value; } }
        public Func<HttpRequest, string> GetUrlSubDirectoryFunction { get { return _getUrlSubDirectory; } set { _getUrlSubDirectory = value; } }

        public string GetUrlPath(HttpRequest httpRequest)
        {
            return zPath.Combine(_cacheDirectory, GetUrlSubPath(httpRequest));
        }

        public string GetUrlSubPath(HttpRequest httpRequest)
        {
            string file = GetUrlFilename(httpRequest);
            string dir = GetUrlSubDirectory(httpRequest);
            if (dir != null)
                file = zPath.Combine(dir, file);
            return file;
        }

        public virtual string GetUrlSubDirectory(HttpRequest httpRequest)
        {
            if (_getUrlSubDirectory != null)
                return _getUrlSubDirectory(httpRequest);
            else
                return null;
        }

        public virtual string GetUrlFilename(HttpRequest httpRequest)
        {
            return zurl.UrlToFileName(httpRequest, _urlFileNameType);
        }

        public static UrlCache Create(XElement xe)
        {
            if (xe.zXPathValue("UseUrlCache").zTryParseAs(false))
            {
                UrlCache urlCache = new UrlCache(xe.zXPathValue("CacheDirectory"));
                urlCache.UrlFileNameType = zurl.GetUrlFileNameType(xe.zXPathValue("CacheUrlFileNameType"));
                return urlCache;
            }
            else
                return null;
        }
    }
}
