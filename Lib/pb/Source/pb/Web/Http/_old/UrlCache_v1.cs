using System;
using System.IO;
using pb.IO;
using pb.Web.old;

namespace pb.Web
{
    public delegate string GetUrlSubDirectoryDelegate_v1(string url, HttpRequestParameters_v1 requestParameters = null);

    public class UrlCache_v1
    {
        protected string _cacheDirectory = null;
        protected UrlFileNameType _urlFileNameType = UrlFileNameType.Path;
        protected GetUrlSubDirectoryDelegate_v1 _getUrlSubDirectory = null;

        public UrlCache_v1(string cacheDirectory, UrlFileNameType urlFileNameType = UrlFileNameType.Path, GetUrlSubDirectoryDelegate_v1 getUrlSubDirectory = null)
        {
            _cacheDirectory = cacheDirectory;
            _urlFileNameType = urlFileNameType;
            _getUrlSubDirectory = getUrlSubDirectory;
        }

        public string CacheDirectory { get { return _cacheDirectory; } }
        public UrlFileNameType UrlFileNameType { get { return _urlFileNameType; } set { _urlFileNameType = value; } }
        public GetUrlSubDirectoryDelegate_v1 GetUrlSubDirectoryFunction { get { return _getUrlSubDirectory; } set { _getUrlSubDirectory = value; } }

        public string GetUrlPath(string url, HttpRequestParameters_v1 requestParameters = null)
        {
            return zPath.Combine(_cacheDirectory, GetUrlSubPath(url, requestParameters));
        }

        public string GetUrlSubPath(string url, HttpRequestParameters_v1 requestParameters = null)
        {
            string file = GetUrlFilename(url, requestParameters);
            string dir = GetUrlSubDirectory(url, requestParameters);
            if (dir != null)
                file = zPath.Combine(dir, file);
            return file;
        }

        public virtual string GetUrlSubDirectory(string url, HttpRequestParameters_v1 requestParameters = null)
        {
            if (_getUrlSubDirectory != null)
                return _getUrlSubDirectory(url, requestParameters);
            else
                return null;
        }

        public virtual string GetUrlFilename(string url, HttpRequestParameters_v1 requestParameters = null)
        {
            return zurl.UrlToFileName(url, _urlFileNameType, httpRequestContent: requestParameters != null ? requestParameters.content : null);
        }
    }
}
