using System;
using pb;
using pb.Web;

namespace Test.Test_Http
{
    public static class Test_Uri
    {
        public static void Test()
        {
            Test_Uri_01("http://www.site.com/toto/tata/index.php?name=search&page=2");
            Test_Uri_01("http://www.site.com/toto/tata/");
            Test_Uri_01("http://cimg.partner.shopping.voila.fr/srv/FR/2801525017fa5500/T/500x500/C/FFFFFF/url/DES-GAULOIS-AUX");
            Test_Uri_01("http://imagizer.imageshack.us/v2/360x600q150/http://pixhst.com/avaxhome/9a/67/002e679a_medium.jpeg");
            //Test_Uri_01("");
            //Test_Uri_01("");
            //Test_Uri_01("");
        }

        public static void Test_Uri_01(string url)
        {
            Uri uri = new Uri(url);
            Trace.WriteLine();
            Trace.WriteLine("url                                   : \"{0}\"", url);
            Trace.WriteLine("uri.Domain                            : \"{0}\"", zurl.GetDomain(url));
            Trace.WriteLine("uri.Host                              : \"{0}\"", uri.Host);
            Trace.WriteLine("uri.Authority                         : \"{0}\"", uri.Authority);
            Trace.WriteLine("uri.DnsSafeHost                       : \"{0}\"", uri.DnsSafeHost);
            Trace.WriteLine("uri.AbsoluteUri                       : \"{0}\"", uri.AbsoluteUri);
            Trace.WriteLine("uri.PathAndQuery                      : \"{0}\"", uri.PathAndQuery);
            Trace.WriteLine("uri.AbsolutePath                      : \"{0}\"", uri.AbsolutePath);
            Trace.WriteLine("uri.LocalPath                         : \"{0}\"", uri.LocalPath);
            Trace.WriteLine("uri.Query                             : \"{0}\"", uri.Query);
            Trace.WriteLine("uri.Segments                          : \"{0}\"", uri.Segments.zToStringValues(", "));

            //url                                   : "http://www.site.com/toto/tata/index.php?name=search&page=2"
            //uri.Host                              : "www.site.com"
            //uri.Authority                         : "www.site.com"
            //uri.DnsSafeHost                       : "www.site.com"
            //uri.AbsoluteUri                       : "http://www.site.com/toto/tata/index.php?name=search&page=2"
            //uri.PathAndQuery                      : "/toto/tata/index.php?name=search&page=2"
            //uri.AbsolutePath                      : "/toto/tata/index.php"
            //uri.LocalPath                         : "/toto/tata/index.php"
            //uri.Query                             : "?name=search&page=2"
            //uri.Segments                          : "/, toto/, tata/, index.php"

            //url                                   : "http://www.site.com/toto/tata/"
            //uri.Host                              : "www.site.com"
            //uri.Authority                         : "www.site.com"
            //uri.DnsSafeHost                       : "www.site.com"
            //uri.AbsoluteUri                       : "http://www.site.com/toto/tata/"
            //uri.PathAndQuery                      : "/toto/tata/"
            //uri.AbsolutePath                      : "/toto/tata/"
            //uri.LocalPath                         : "/toto/tata/"
            //uri.Query                             : ""
            //uri.Segments                          : "/, toto/, tata/"

        }
    }
}
