using System;
using System.Xml.Linq;
using pb.Data.Xml;
using pb.IO;

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
        protected Func<HttpRequest_v3, string> _getUrlSubDirectory_v2 = null;

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
            return GetUrlPathResult(GetUrlSubPath(httpRequest), subDirectory);
        }

        public UrlCachePathResult GetUrlPathResult(HttpRequest_v3 httpRequest, string subDirectory = null)
        {
            return GetUrlPathResult(GetUrlSubPath(httpRequest), subDirectory);
        }

        //public UrlCachePathResult GetUrlPathResult(HttpRequest httpRequest, string subDirectory = null)
        private UrlCachePathResult GetUrlPathResult(string subPath, string subDirectory = null)
        {
            //string subPath = GetUrlSubPath(httpRequest);
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
            //string file = zurl.UrlToFileName(httpRequest, _urlFileNameType);
            string file = zurl.UrlToFileName(httpRequest.Url, _urlFileNameType, httpRequestContent: httpRequest.Content);

            if (!_indexedFile)
            {
                string dir = _getUrlSubDirectory?.Invoke(httpRequest);
                if (dir != null)
                    file = zPath.Combine(dir, file);
            }

            return file;
        }

        public string GetUrlSubPath(HttpRequest_v3 httpRequest)
        {
            //string file = zurl.UrlToFileName(httpRequest, _urlFileNameType);
            //string file = zurl.UrlToFileName(httpRequest.Url, _urlFileNameType, httpRequestContent: httpRequest.Content);
            string file = zurl.UrlToFileName(httpRequest.Url, _urlFileNameType);

            if (!_indexedFile)
            {
                string dir = _getUrlSubDirectory_v2?.Invoke(httpRequest);
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

        public static UrlCache CreateIndexedCache(string directory, UrlFileNameType urlFileNameType = UrlFileNameType.Host | UrlFileNameType.Path | UrlFileNameType.Ext | UrlFileNameType.Query)
        {
            if (directory == null)
                return null;
            UrlCache urlCache = new UrlCache(directory);
            urlCache._urlFileNameType = urlFileNameType;
            urlCache._indexedFile = true;
            urlCache._saveRequest = true;
            return urlCache;
        }
    }
}
