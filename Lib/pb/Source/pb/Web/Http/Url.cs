using System;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Web;
using pb.IO;

namespace pb.Web.Http
{
    [Flags]
    public enum UrlFileNameType
    {
        /// <summary>
        /// http://www.site.com/toto/tata/index.php?name=search => www.site.com
        /// </summary>
        Host     = 0x0001,
        /// <summary>
        /// http://www.site.com/toto/tata/index.php?name=search => index.php
        /// </summary>
        FileName = 0x0002,  // last segment
        /// <summary>
        /// http://www.site.com/toto/tata/index.php?name=search => toto_tata_index.php
        /// </summary>
        Path     = 0x0004,
        Ext      = 0x0008,
        /// <summary>
        /// http://www.site.com/toto/tata/index.php?name=search => name=search.php
        /// </summary>
        Query    = 0x0010,
        /// <summary>
        /// name1=value1&name2[]=value2&name2[]=value3 => name1=value1_name2[]=value2_name2[]=value3.ext
        /// </summary>
        Content  = 0x0020
    }

    public static class zurl
    {
        //private static Regex __rgBadFilenameChars = new Regex(@"/|\?|%|&|\\|:", RegexOptions.Compiled);
        private static Regex __rgMultiUnderline = new Regex("_{2,}", RegexOptions.Compiled);
        private static int __maxFilenameLength = 150;

        //public static string UrlToFileName(string url, UrlFileNameType type, string ext = null, HttpRequestParameters requestParameters = null)
        //{
        //    string httpRequestContent = null;
        //    if (requestParameters != null)
        //        httpRequestContent = requestParameters.content;
        //    return UrlToFileName(url, type, ext, httpRequestContent);
        //}

        //public static string UrlToFileName(HttpRequest request, UrlFileNameType type, string ext = null)
        //{
        //    return UrlToFileName(request.Url, type, ext, request.Content);
        //}

        public static string UrlToFileName(string url, UrlFileNameType type, string ext = null, string httpRequestContent = null)
        {
            // source from Http.UrlToFileName()

            // url                                   : "http://www.site.com/toto/tata/index.php?name=search&page=2"
            // uri.Host                              : "www.site.com"
            // uri.Authority                         : "www.site.com"
            // uri.DnsSafeHost                       : "www.site.com"
            // uri.AbsoluteUri                       : "http://www.site.com/toto/tata/index.php?name=search&page=2"
            // uri.PathAndQuery                      : "/toto/tata/index.php?name=search&page=2"
            // uri.AbsolutePath                      : "/toto/tata/index.php"
            // uri.LocalPath                         : "/toto/tata/index.php"
            // uri.Query                             : "?name=search&page=2"
            // uri.Segments                          : "/, toto/, tata/, index.php"

            // url                                   : "http://www.site.com/toto/tata/"
            // uri.Host                              : "www.site.com"
            // uri.Authority                         : "www.site.com"
            // uri.DnsSafeHost                       : "www.site.com"
            // uri.AbsoluteUri                       : "http://www.site.com/toto/tata/"
            // uri.PathAndQuery                      : "/toto/tata/"
            // uri.AbsolutePath                      : "/toto/tata/"
            // uri.LocalPath                         : "/toto/tata/"
            // uri.Query                             : ""
            // uri.Segments                          : "/, toto/, tata/"

            // uri.GetComponents(UriComponents.UserInfo)
            // uri.GetLeftPart(UriPartial.Scheme)

            if (url == null)
                return null;

            string file = "";
            //bool queryOrContent = false;
            Uri uri = new Uri(url);

            if ((type & UrlFileNameType.Host) == UrlFileNameType.Host)
            {
                file = uri.Host;
            }

            if ((type & UrlFileNameType.Path) == UrlFileNameType.Path)
            {
                if (file != "")
                    file += "_";
                //file += HttpUtility.UrlDecode(uri.AbsolutePath);
                string urlPath = HttpUtility.UrlDecode(uri.AbsolutePath);
                //urlExt = zPath.GetExtension(urlPath);
                if (urlPath.StartsWith("/"))
                    urlPath = urlPath.Substring(1);
                if (urlPath.EndsWith("/"))
                    urlPath = urlPath.Substring(0, urlPath.Length - 1);

                if ((type & UrlFileNameType.Ext) != UrlFileNameType.Ext)
                {
                    // attention with zpath.PathSetExt() / => \
                    urlPath = zpath.PathSetExtension(urlPath, null);
                }
                file += urlPath;
            }
            else if ((type & UrlFileNameType.FileName) == UrlFileNameType.FileName)
            {
                if (file != "")
                    file += "_";
                //file += HttpUtility.UrlDecode(uri.Segments[uri.Segments.Length - 1]);
                //if (file.EndsWith("/"))
                //    file = file.Substring(0, file.Length - 1);
                string urlFilename = HttpUtility.UrlDecode(uri.Segments[uri.Segments.Length - 1]);
                //urlExt = zPath.GetExtension(urlFilename);
                if (urlFilename.EndsWith("/"))
                    urlFilename = urlFilename.Substring(0, urlFilename.Length - 1);
                if ((type & UrlFileNameType.Ext) != UrlFileNameType.Ext)
                {
                    // attention with zpath.PathSetExt() / => \
                    urlFilename = zpath.PathSetExtension(urlFilename, null);
                }
                file += urlFilename;
            }

            if ((type & UrlFileNameType.Query) == UrlFileNameType.Query)
            {
                if (file != "")
                    file += "_";
                file += HttpUtility.UrlDecode(uri.Query);
                //queryOrContent = true;
            }

            if ((type & UrlFileNameType.Content) == UrlFileNameType.Content)
            {
                //if (requestParameters != null && requestParameters.content != null)
                if (httpRequestContent != null)
                {
                    if (file != "")
                        file += "_";
                    //file += requestParameters.content;
                    file += httpRequestContent;
                }
                //queryOrContent = true;
            }

            //file = __rgBadFilenameChars.Replace(file, "_");
            file = zfile.ReplaceBadFilenameChars(file, "_");
            //file = file.Replace('/', '_');
            //file = file.Replace('?', '_');
            //file = file.Replace('%', '_');
            //file = file.Replace('&', '_');
            file = file.Trim('_');
            file = __rgMultiUnderline.Replace(file, "_");
            //if (ext != null)
            //    file = zpath.PathSetExt(file, ext);
            //else if (!zPath.HasExtension(file))
            //    file = zpath.PathSetExt(file, ".html");

            if (file == "")
                throw new PBException("error can't generate filename from url \"{0}\" of type {1}", url, type);

            if (ext == null)
            {
                ext = zPath.GetExtension(uri.AbsolutePath);
                // modif le 09/11/2015 appel Http.GetContentTypeFromFileExtension() pour vérifier le type d'extension
                if (HttpTools.GetContentTypeFromFileExtension(ext) == null)
                {
                    file += ext;
                    ext = "";
                }
                ////////////// annulation modif le 24/07/2016 ajout else pour ne pas ajouter ".html" si Http.GetContentTypeFromFileExtension(ext) = null
                if (ext == "")
                    ext = ".html";
                //else if (!queryOrContent)
                //    ext = null;
            }

            if (ext != null)
            {
                //if (!queryOrContent)
                //    file = zpath.PathSetExt(file, ext);
                //else
                //    file += ext;
                if (!file.EndsWith(ext))
                    file += ext;
            }
            if (file.Length > __maxFilenameLength)
            {
                ext = zPath.GetExtension(file);
                string filename = zPath.GetFileNameWithoutExtension(file);
                int i = filename.LastIndexOf('.');
                if (i != -1 && filename.Length - i < 10)
                {
                    ext = filename.Substring(i) + ext;
                    filename = filename.Substring(0, i);
                }
                file = filename.Substring(0, filename.Length - (file.Length - __maxFilenameLength)) + ext;
            }

            return file;
        }

        public static string GetUrlFileType(string url)
        {
            Uri uri = new Uri(url);
            return zPath.GetExtension(uri.Segments[uri.Segments.Length - 1]);
        }

        private static Regex __domainRegex = new Regex(@"[^.]+\.[^.]+$", RegexOptions.Compiled);
        public static string GetDomain(string url)
        {
            return __domainRegex.Match(new Uri(url).Host).Value;
        }

        //public static string GetExtension(string url)
        //{
        //    return zPath.GetExtension(new Uri(url).AbsolutePath);
        //}

        //public static string GetFileName(string url)
        //{
        //    return zPath.GetFileName(new Uri(url).AbsolutePath);
        //}

        // example : http://toto.com/zozo/zuzu/zaza.txt => /zozo/zuzu/zaza.txt
        public static string GetAbsolutePath(string url)
        {
            return new Uri(url).AbsolutePath;
        }

        public static string RemoveFragment(string url)
        {
            // url syntax : scheme:[//[user:password@]host[:port]][/]path[?query][#fragment]    (https://en.wikipedia.org/wiki/Uniform_Resource_Locator)
            if (url == null)
                return url;
            return new Uri(url).GetLeftPart(UriPartial.Query);
        }

        public static string GetUrl(string baseUrl, string url)
        {
            if (url == null)
                return null;
            Uri uri;
            if (baseUrl != null)
            {
                Uri baseUri = new Uri(baseUrl);
                uri = new Uri(baseUri, url);
            }
            else
                uri = new Uri(url);
            return uri.AbsoluteUri;
        }

        public static NameValueCollection GetQueryValues(string url)
        {
            Uri uri = new Uri(url);
            string query = uri.Query;
            return System.Web.HttpUtility.ParseQueryString(query);
        }

        public static UrlFileNameType GetUrlFileNameType(string urlFileNameType)
        {
            // Host, Path, Ext, Query, Content
            UrlFileNameType type = 0;
            foreach (string s in urlFileNameType.Split(','))
            {
                switch (s.Trim().ToLowerInvariant())
                {
                    case "host":
                        type |= UrlFileNameType.Host;
                        break;
                    case "filename":
                        type |= UrlFileNameType.FileName;
                        break;
                    case "path":
                        type |= UrlFileNameType.Path;
                        break;
                    case "ext":
                        type |= UrlFileNameType.Ext;
                        break;
                    case "query":
                        type |= UrlFileNameType.Query;
                        break;
                    case "content":
                        type |= UrlFileNameType.Content;
                        break;
                }
            }
            return type;
        }

        public static bool CheckUrl(string url)
        {
            try
            {
                new Uri(url);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
