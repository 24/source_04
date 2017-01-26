using System.Collections.Specialized;
using System.Net;
using System.Text;

namespace pb.Web.Http
{
    public enum HttpRequestMethod
    {
        Get = 0,
        Post
    }

    public class HttpRequest
    {
        public string Url;
        public HttpRequestMethod Method = HttpRequestMethod.Get;
        public string Referer = null;
        public string Content = null;
        public bool ReloadFromWeb = false;
        //public bool CacheFile = false;
        public UrlCachePathResult UrlCachePath = null;
    }

    public class HttpRequestParameters
    {
        //public bool UseWebClient = false;                              // static use System.Net.WebClient or System.Net.WebRequest
        public Encoding Encoding = null;                                 // static
        //public HttpRequestMethod Method = HttpRequestMethod.Get;       // request
        public string UserAgent = "pib/0.1";                             // static   "Mozilla/5.0 Pib";
        public string Accept = null;                                     // request
        //public string Referer = null;                                  // request
        public DecompressionMethods? AutomaticDecompression = null;      // static
        public NameValueCollection Headers = new NameValueCollection();  // request
        public string ContentType = "application/x-www-form-urlencoded"; // static    valeur par defaut car obligatoire sur certain serveur (ex: http://www.handeco.org/fournisseurs/rechercher)
        //public string Content = null;                                  // request
        public CookieContainer Cookies = new CookieContainer();          // static
        public bool Expect100Continue = false;                           // false permet d'éviter que le content soit envoyé séparément avec Expect: 100-continue dans l'entete du 1er paquet
    }
}
