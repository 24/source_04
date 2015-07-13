using System;
using System.Collections.Specialized;
using pb;
using pb.Data.Mongo;
using pb.Web;

namespace Test.Test_Web
{
    public static class Test_Url
    {
        public static void Test_Uri_01(string url)
        {
            //http://www.telechargement-plus.com/index.php?name=search
            //string url = "http://www.telechargement-plus.com/toto/tata/index.php?name=search";
            Trace.WriteLine("url                  \"{0}\"", url);
            Uri uri = new Uri(url);
            Trace.WriteLine("uri.OriginalString   \"{0}\"", uri.OriginalString);
            Trace.WriteLine("uri.AbsoluteUri      \"{0}\"", uri.AbsoluteUri);
            Trace.WriteLine("uri.PathAndQuery     \"{0}\"", uri.PathAndQuery);
            Trace.WriteLine("uri.AbsolutePath     \"{0}\"", uri.AbsolutePath);
            Trace.WriteLine("uri.LocalPath        \"{0}\"", uri.LocalPath);
            Trace.WriteLine("uri.Query            \"{0}\"", uri.Query);
            Trace.WriteLine("uri.Segments         \"{0}\"", uri.Segments.zToStringValues());
            Trace.WriteLine();
            Trace.WriteLine("uri.Authority        \"{0}\"", uri.Authority);
            Trace.WriteLine("uri.DnsSafeHost      \"{0}\"", uri.DnsSafeHost);
            Trace.WriteLine("uri.Host             \"{0}\"", uri.Host);
            Trace.WriteLine("uri.HostNameType     \"{0}\"", uri.HostNameType);
            Trace.WriteLine();
            Trace.WriteLine("uri.IsAbsoluteUri    \"{0}\"", uri.IsAbsoluteUri);
            Trace.WriteLine("uri.IsDefaultPort    \"{0}\"", uri.IsDefaultPort);
            Trace.WriteLine("uri.IsFile           \"{0}\"", uri.IsFile);
            Trace.WriteLine("uri.IsLoopback       \"{0}\"", uri.IsLoopback);
            Trace.WriteLine("uri.IsUnc            \"{0}\"", uri.IsUnc);
            Trace.WriteLine("uri.UserEscaped      \"{0}\"", uri.UserEscaped);
            Trace.WriteLine();
            Trace.WriteLine("uri.Port             \"{0}\"", uri.Port);
            Trace.WriteLine("uri.Scheme           \"{0}\"", uri.Scheme);
            Trace.WriteLine();
            Trace.WriteLine("uri.Fragment         \"{0}\"", uri.Fragment);
            Trace.WriteLine("uri.UserInfo         \"{0}\"", uri.UserInfo);
        }

        public static void Test_Query_01(string url)
        {
            Trace.WriteLine("url \"{0}\"", url);
            Uri uri = new Uri(url);
            string query = uri.Query;
            NameValueCollection values = System.Web.HttpUtility.ParseQueryString(query);
            foreach (string name in values)
            {
                Trace.WriteLine("name \"{0}\" = \"{1}\"", name, values[name]);
            }
            Trace.WriteLine("ToString() \"{0}\"", values.ToString());
        }

        public static void Test_UriBuilder_01(string url)
        {
            Trace.WriteLine("url \"{0}\"", url);
            UriBuilder uriBuilder = new UriBuilder(url);
            NameValueCollection values = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);
            string name = "ss";
            Trace.WriteLine("remove query value \"{0}\"", name);
            values.Remove(name);
            uriBuilder.Query = values.ToString();
            Trace.WriteLine("url \"{0}\"", uriBuilder.ToString());
        }

        public static void Test_PBUriBuilder_01(string url)
        {
            Trace.WriteLine("url \"{0}\"", url);
            PBUriBuilder uriBuilder = new PBUriBuilder(url);
            //Trace.WriteLine("port {0}", uriBuilder.Port);
            //uriBuilder.Port = -1;
            string name = "ss";
            Trace.WriteLine("remove query value \"{0}\"", name);
            uriBuilder.RemoveQueryValue(name);
            Trace.WriteLine("url \"{0}\"", uriBuilder.ToString());
        }

        //public static void Test_UrlToFileName_01(string url, string ext, UrlFileNameType type = UrlFileNameType.FileName)
        //{
        //    Trace.WriteLine("url  \"{0}\"", url);
        //    Trace.WriteLine("ext  \"{0}\"", ext);
        //    Trace.WriteLine("type \"{0}\"", type);
        //    string file = Http_new.UrlToFileName(url, ext, type);
        //    Trace.WriteLine("file \"{0}\"", file);
        //}

        public static void Test_UrlToFileName_01(string url, string ext, UrlFileNameType type = UrlFileNameType.FileName)
        {
            Trace.WriteLine("url  \"{0}\"", url);
            Trace.WriteLine("ext  \"{0}\"", ext);
            Trace.WriteLine("type \"{0}\"", type);
            //string file = Http_new.UrlToFileName(url, ext, type);
            string file = zurl.UrlToFileName(new HttpRequest { Url = url }, type, ext);
            Trace.WriteLine("file \"{0}\"", file);
        }
    }
}
