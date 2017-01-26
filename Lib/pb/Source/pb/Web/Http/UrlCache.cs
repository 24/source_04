using System;
using System.Xml.Linq;
using pb.Data.Xml;
using pb.IO;

// to do
//  gérer loadFromWebDate du cache

namespace pb.Web.Http
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
        protected bool _indexedFile = false;
        protected bool _saveRequest = false;
        protected Func<HttpRequest, string> _getUrlSubDirectory = null;

        public UrlCache(string cacheDirectory)
        {
            _cacheDirectory = cacheDirectory;
        }

        public string CacheDirectory { get { return _cacheDirectory; } }
        public UrlFileNameType UrlFileNameType { get { return _urlFileNameType; } set { _urlFileNameType = value; } }
        public bool IndexedFile { get { return _indexedFile; } set { _indexedFile = value; } }
        public bool SaveRequest { get { return _saveRequest; } set { _saveRequest = value; } }
        public Func<HttpRequest, string> GetUrlSubDirectory { get { return _getUrlSubDirectory; } set { _getUrlSubDirectory = value; } }

        public string GetUrlPath(HttpRequest httpRequest)
        {
            return zPath.Combine(_cacheDirectory, GetUrlSubPath(httpRequest));
        }

        public UrlCachePathResult GetUrlPathResult(HttpRequest httpRequest, string subDirectory = null)
        {
            string subPath = GetUrlSubPath(httpRequest);
            string path;
            if (_indexedFile)
                path = zfile.GetNewIndexedFileName(_cacheDirectory) + "_" + subPath;
            else
            {
                if (subDirectory != null)
                    subPath = zPath.Combine(subDirectory, subPath);
                path = zPath.Combine(_cacheDirectory, subPath);
            }
            return new UrlCachePathResult { Path = path, SubPath = subPath };
        }

        public string GetUrlSubPath(HttpRequest httpRequest)
        {
            //string file = GetUrlFilename(httpRequest);
            string file = zurl.UrlToFileName(httpRequest, _urlFileNameType);

            if (!_indexedFile)
            {
                //string dir = _GetUrlSubDirectory(httpRequest);
                string dir = _getUrlSubDirectory?.Invoke(httpRequest);
                if (dir != null)
                    file = zPath.Combine(dir, file);
            }

            return file;
        }

        //private string _GetUrlSubDirectory(HttpRequest httpRequest)
        //{
        //    if (_getUrlSubDirectory != null)
        //        return _getUrlSubDirectory(httpRequest);
        //    else
        //        return null;
        //}

        //private string GetUrlFilename(HttpRequest httpRequest)
        //{
        //    return zurl.UrlToFileName(httpRequest, _urlFileNameType);
        //}

        public static UrlCache Create(XElement xe)
        {
            if (xe != null && xe.zXPathValue("UseUrlCache").zTryParseAs(false))
            {
                UrlCache urlCache = new UrlCache(xe.zXPathExplicitValue("CacheDirectory"));
                urlCache._urlFileNameType = zurl.GetUrlFileNameType(xe.zXPathValue("CacheUrlFileNameType", "Path"));
                urlCache._indexedFile = xe.zXPathValue("IndexedFile").zTryParseAs(false);
                urlCache._saveRequest = xe.zXPathValue("CacheSaveRequest").zTryParseAs(false);
                return urlCache;
            }
            else
                return null;
        }
    }
}
