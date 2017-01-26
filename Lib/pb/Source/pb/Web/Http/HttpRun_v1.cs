using pb.Web.Html;
using System.Drawing;
using System.IO;
using System.Xml.Linq;

namespace pb.Web.Http
{
    public static class HttpRun_v1
    {
        private static XmlDocumentSourceType __xmlDocumentSourceType = XmlDocumentSourceType.NoSource;
        private static XDocument __xDocument = null;

        // http
        //private static HttpManager __httpManager = new HttpManager();
        private static Http __http = null;

        // xml file
        private static string __xmlFile = null;

        // html
        private static string __html = null;

        public static Http Http { get { return __http; } }

        public static Http Load(string url, HttpRequestParameters requestParameters = null, string exportFile = null, bool setExportFileExtension = false)
        {
            return Load(new HttpRequest { Url = url }, requestParameters, exportFile, setExportFileExtension);
        }

        public static Http Load(HttpRequest httpRequest, HttpRequestParameters requestParameters = null, string exportFile = null, bool setExportFileExtension = false)
        {
            RazSource();
            __xmlDocumentSourceType = XmlDocumentSourceType.Http;
            //__http = __httpManager.Load(httpRequest, requestParameters);
            __http = HttpManager.CurrentHttpManager.Load(httpRequest, requestParameters, exportFile, setExportFileExtension);
            return __http;
        }

        public static bool LoadToFile(string url, string file, bool exportRequest = false, HttpRequestParameters requestParameters = null)
        {
            return HttpManager.CurrentHttpManager.LoadToFile(new HttpRequest { Url = url }, file, exportRequest, requestParameters);
        }

        public static bool LoadToFile(HttpRequest httpRequest, string file, bool exportRequest = false, HttpRequestParameters requestParameters = null)
        {
            return HttpManager.CurrentHttpManager.LoadToFile(httpRequest, file, exportRequest, requestParameters);
        }

        public static Image LoadImage(string url, HttpRequestParameters requestParameters = null)
        {
            return HttpManager.CurrentHttpManager.LoadImage(new HttpRequest { Url = url }, requestParameters);
        }

        public static Image LoadImage(HttpRequest httpRequest, HttpRequestParameters requestParameters = null)
        {
            return HttpManager.CurrentHttpManager.LoadImage(httpRequest, requestParameters);
        }

        public static void LoadXmlFile(string file)
        {
            RazSource();
            __xmlDocumentSourceType = XmlDocumentSourceType.XmlFile;
            __xmlFile = file;
        }

        public static void LoadHtmlString(string html)
        {
            RazSource();
            __xmlDocumentSourceType = XmlDocumentSourceType.XmlFile;
            __html = html;
        }

        private static void RazSource()
        {
            __xmlDocumentSourceType = XmlDocumentSourceType.NoSource;
            __xDocument = null;
            if (__http != null)
            {
                __http.Close();
                __http = null;
            }
            __xmlFile = null;
            __html = null;
        }

        public static XDocument GetXDocument()
        {
            if (__xDocument == null)
            {
                if (__xmlDocumentSourceType == XmlDocumentSourceType.Http)
                {
                    //__xDocument = __http.zGetXDocument();
                    __xDocument = HttpManager.CurrentHttpManager.GetXDocument(__http);
                }
                else if (__xmlDocumentSourceType == XmlDocumentSourceType.XmlFile)
                    __xDocument = XDocument.Load(__xmlFile);
                else if (__xmlDocumentSourceType == XmlDocumentSourceType.HtmlString)
                {
                    if (__html != null)
                    {
                        using (StringReader sr = new StringReader(__html))
                        {
                            HtmlToXml_v2 hx = new HtmlToXml_v2(sr);
                            //hx.ReadCommentInText = _webReadCommentInText;
                            __xDocument = hx.GenerateXDocument();
                        }

                        //if (_trace.TraceLevel >= 2)
                        //{
                        //    string sPath = GetNewFileName();
                        //    if (_xDocument != null && sPath != null) _xDocument.Save(sPath + "_LoadHtmlString.xml");
                        //}
                    }
                }
            }
            return __xDocument;
        }
    }
}
