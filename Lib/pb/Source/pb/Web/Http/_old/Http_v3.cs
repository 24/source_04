using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;

namespace pb.Web.old
{
    public class HttpRequestParameters_v1
    {
        public bool useWebClient = false;                                // static use System.Net.WebClient or System.Net.WebRequest
        public Encoding encoding = null;                                 // static
        public HttpRequestMethod method = HttpRequestMethod.Get;         // request
        //public string userAgent = "Pib";                                 // static
        // problème avec userAgent = "Pib" par exemple l'image http://www.babelio.com/couv/11657_639334.jpeg ne se charge pas
        public string userAgent = "Mozilla/5.0 Pib";                     // static
        public string accept = null;                                     // request
        public string referer = null;                                    // request
        public DecompressionMethods? automaticDecompression = null;      // static
        public NameValueCollection headers = new NameValueCollection();  // request
        //public string contentType = null;                              // request
        public string contentType = "application/x-www-form-urlencoded"; // static    valeur par defaut car obligatoire sur certain serveur (ex: http://www.handeco.org/fournisseurs/rechercher)
        public string content = null;                                    // request
        public CookieContainer cookies = new CookieContainer();          // static
        public bool Expect100Continue = false;                           // false permet d'éviter que le content soit envoyé séparément avec Expect: 100-continue dans l'entete du 1er paquet
    }

    public static class Http_v3
    {
        private static Http __http = null;

        public static Http Http { get { return __http; } }

        public static bool LoadUrl(string url, HttpRequestParameters_v1 requestParameters = null)
        {
            __http = HttpManager.CurrentHttpManager.Load(CreateHttpRequest(url, requestParameters), CreateHttpRequestParameters(requestParameters));
            return __http != null;
        }

        public static bool LoadToFile(string url, string file, bool exportRequest = false, HttpRequestParameters_v1 requestParameters = null)
        {
            return HttpManager.CurrentHttpManager.LoadToFile(CreateHttpRequest(url, requestParameters), file, exportRequest, CreateHttpRequestParameters(requestParameters));
        }

        public static HttpRequest CreateHttpRequest(string url, HttpRequestParameters_v1 requestParameters)
        {
            HttpRequest httpRequest = new HttpRequest();
            httpRequest.Url = url;
            if (requestParameters != null)
            {
                httpRequest.Method = requestParameters.method;
                httpRequest.Content = requestParameters.content;
                httpRequest.Referer = requestParameters.referer;
            }
            return httpRequest;
        }

        public static HttpRequestParameters CreateHttpRequestParameters(HttpRequestParameters_v1 requestParameters)
        {
            HttpRequestParameters requestParameters_new = new HttpRequestParameters();
            if (requestParameters != null)
            {
                requestParameters_new.Accept = requestParameters.accept;
                requestParameters_new.AutomaticDecompression = requestParameters.automaticDecompression;
                requestParameters_new.ContentType = requestParameters.contentType;
                requestParameters_new.Cookies = requestParameters.cookies;
                requestParameters_new.Encoding = requestParameters.encoding;
                requestParameters_new.Expect100Continue = requestParameters.Expect100Continue;
                requestParameters_new.Headers = requestParameters.headers;
                requestParameters_new.UserAgent = requestParameters.userAgent;
            }
            return requestParameters_new;
        }
    }
}
