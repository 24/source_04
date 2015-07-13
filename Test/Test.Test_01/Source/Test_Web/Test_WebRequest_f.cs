using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Cache;

namespace Test_Web
{
    static partial class w
    {
        public static void Test_WebRequest_01(string url)
        {
            // create HttpWebRequest or FileWebRequest
            WebRequest req = WebRequest.Create(url);
            _tr.WriteLine("Test_WebRequest_01");
            _tr.WriteLine("  url : \"{0}\"", url);
            _tr.WriteLine("  WebRequest.Create(url) : \"{0}\"", req);
        }

        public static void Test_WebRequest_02(string url)
        {
            // create HttpWebRequest or FileWebRequest
            WebRequest req = WebRequest.Create(url);
            _tr.WriteLine("Test_WebRequest_02");
            _tr.WriteLine("  url : \"{0}\"", url);
            _tr.WriteLine("  WebRequest.Create(url) : \"{0}\"", req);
            using (WebResponse response = req.GetResponse())
            {
                _tr.WriteLine("  WebRequest.GetResponse() : \"{0}\"", response);
                _tr.WriteLine("    ContentLength   : {0}", response.ContentLength);
                _tr.WriteLine("    ContentType     : \"{0}\"", response.ContentType);
                _tr.WriteLine("    ResponseUri     : \"{0}\"", response.ResponseUri);
                _tr.WriteLine("    Headers         :  \"{0}\"", response.Headers.GetType());
                foreach (string header in response.Headers)
                    _tr.WriteLine("      {0} = {1}", header, response.Headers[header]);
                if (response is HttpWebResponse)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    _tr.WriteLine("  HttpWebResponse :");
                    _tr.WriteLine("    CharacterSet      : \"{0}\"", httpResponse.CharacterSet);
                    _tr.WriteLine("    ContentEncoding   : \"{0}\"", httpResponse.ContentEncoding);
                    _tr.WriteLine("    ContentLength     : {0}", httpResponse.ContentLength);
                    _tr.WriteLine("    ContentType       : \"{0}\"", httpResponse.ContentType);
                    _tr.WriteLine("    Cookies           : \"{0}\"", httpResponse.Cookies.GetType());
                    _tr.WriteLine("    Headers           : \"{0}\"", httpResponse.Headers.GetType());
                    _tr.WriteLine("    LastModified      : {0:dd/MM/yyyy HH:mm:ss}", httpResponse.LastModified);
                    _tr.WriteLine("    Method            : \"{0}\"", httpResponse.Method);
                    _tr.WriteLine("    ProtocolVersion   : \"{0}\"", httpResponse.ProtocolVersion);
                    _tr.WriteLine("    ResponseUri       : \"{0}\"", httpResponse.ResponseUri);
                    _tr.WriteLine("    Server            : \"{0}\"", httpResponse.Server);
                    _tr.WriteLine("    StatusCode        : {0}", httpResponse.StatusCode);
                    _tr.WriteLine("    StatusDescription : \"{0}\"", httpResponse.StatusDescription);
                    
                }
                if (response is FileWebResponse)
                {
                    FileWebResponse fileResponse = (FileWebResponse)response;
                    _tr.WriteLine("  FileWebResponse :");
                }
            }
        }

        public static void Test_WebRequest_03(string url)
        {
            // create HttpWebRequest or FileWebRequest
            WebRequest req = WebRequest.Create(url);
            HttpRequestCachePolicy cachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.Default);
            req.CachePolicy = cachePolicy;
            // RequestCachePolicy
            _tr.WriteLine("Test_WebRequest_03");
            _tr.WriteLine("  url : \"{0}\"", url);
            _tr.WriteLine("  WebRequest.Create(url) : \"{0}\"", req);
        }

    }
}
